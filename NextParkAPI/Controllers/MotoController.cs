using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextParkAPI.Data;
using NextParkAPI.Models;
using NextParkAPI.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace NextParkAPI.Controllers;


[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class MotoController : ControllerBase
{
    private readonly NextParkContext _context;
    private readonly OracleDbService _oracle;

    public MotoController(NextParkContext context, OracleDbService oracle)
    {
        _context = context;
        _oracle = oracle;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar motos",
        Description = "Retorna uma página de motos cadastradas ordenadas pelo identificador." )]
    [ProducesResponseType(typeof(PagedResponse<Moto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<Moto>>> GetMotos([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return BadRequest("Os parâmetros de paginação devem ser maiores que zero.");
        }

        var query = _context.Motos.AsNoTracking().OrderBy(m => m.IdMoto);
        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        var response = new PagedResponse<Moto>(items, totalCount, pageNumber, pageSize);
        AddCollectionLinks(response, pageNumber, pageSize);

        return Ok(response);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Obter moto",
        Description = "Busca uma moto pelo identificador e retorna seus dados com enlaces HATEOAS." )]
    [ProducesResponseType(typeof(ResourceResponse<Moto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResourceResponse<Moto>>> GetMoto(int id)
    {
        var moto = await _context.Motos.AsNoTracking().FirstOrDefaultAsync(m => m.IdMoto == id);
        if (moto == null) return NotFound();
        var response = CreateResourceResponse(moto);
        return Ok(response);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastrar moto (via procedure)")]
    [ProducesResponseType(typeof(ResourceResponse<Moto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResourceResponse<Moto>>> CreateMoto(Moto moto)
    {
        var plsql = "BEGIN pkg_nextpark_core.prc_moto_criar(:p_id_moto,:p_placa,:p_modelo,:p_status,:p_id_vaga); END;";

        var pars = new[]
        {
        new OracleParameter("p_id_moto", OracleDbType.Int32)   { Value = moto.IdMoto },
        new OracleParameter("p_placa",   OracleDbType.Varchar2){ Value = moto.NrPlaca },
        new OracleParameter("p_modelo",  OracleDbType.Varchar2){ Value = moto.NmModelo },
        new OracleParameter("p_status",  OracleDbType.Char)    { Value = moto.StMoto },
        new OracleParameter("p_id_vaga", OracleDbType.Int32)   { Value = moto.IdVaga }
    };

        try
        {
            await _oracle.ExecProcAsync(plsql, pars);
        }
        catch (OracleException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        var created = await _context.Motos.AsNoTracking()
                            .FirstAsync(m => m.IdMoto == moto.IdMoto);

        var response = CreateResourceResponse(created);
        return CreatedAtAction(nameof(GetMoto),
            new { version = GetCurrentApiVersion(), id = created.IdMoto }, response);
    }

    [HttpPost("{id}/status")]
    [SwaggerOperation(
        Summary = "Atualizar Status da moto",
        Description = "Atualiza a moto pelo id e status.")]
    public async Task<IActionResult> MudarStatus(int id, [FromQuery] string novoStatus)
    {
        var plsql = "BEGIN pkg_nextpark_core.prc_moto_mudar_status(:p_id,:p_st); END;";
        await _oracle.ExecProcAsync(plsql,
            new OracleParameter("p_id", OracleDbType.Int32) { Value = id },
            new OracleParameter("p_st", OracleDbType.Char) { Value = novoStatus }
        );
        return NoContent();
    }

    [HttpGet("{id}/json")]
    [SwaggerOperation(
        Summary = "JSON automatico",
        Description = "JSON gerado pelo meu script de Banco de dados")]
    public async Task<ActionResult> MotoJson(int id)
    {
        var sql = "SELECT pkg_nextpark_json.fn_moto_json(:p_id) FROM dual";
        var json = await _oracle.ExecFunctionScalarClobAsync(sql,
            new OracleParameter("p_id", OracleDbType.Int32) { Value = id });
        return Content(json ?? "{}", "application/json");
    }

    [HttpGet("dataset")]
    public async Task<ActionResult> DatasetJson()
    {
        var sql = "SELECT pkg_nextpark_json.fn_dataset_completo FROM dual";
        var json = await _oracle.ExecFunctionScalarClobAsync(sql);
        return Content(json ?? "[]", "application/json");
    }


    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualizar moto",
        Description = "Atualiza completamente os dados de uma moto existente." )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateMoto(int id, Moto moto)
    {
        if (id != moto.IdMoto) return BadRequest();
        var exists = await _context.Motos.CountAsync(m => m.IdMoto == id) > 0;
        if (!exists) return NotFound();
        _context.Entry(moto).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Excluir moto",
        Description = "Remove uma moto do pátio a partir do identificador informado." )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteMoto(int id)
    {
        var moto = await _context.Motos.FindAsync(id);
        if (moto == null) return NotFound();
        _context.Motos.Remove(moto);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private void AddCollectionLinks(PagedResponse<Moto> response, int pageNumber, int pageSize)
    {
        var version = GetCurrentApiVersion();
        AddLink(response.Links, Url.Action(nameof(GetMotos), new { version, pageNumber, pageSize }), "self", "GET");
        if (pageNumber > 1)
        {
            AddLink(response.Links, Url.Action(nameof(GetMotos), new { version, pageNumber = pageNumber - 1, pageSize }), "previous", "GET");
        }

        if (pageNumber < response.TotalPages)
        {
            AddLink(response.Links, Url.Action(nameof(GetMotos), new { version, pageNumber = pageNumber + 1, pageSize }), "next", "GET");
        }

        AddLink(response.Links, Url.Action(nameof(CreateMoto), new { version }), "create", "POST");
    }


    private ResourceResponse<Moto> CreateResourceResponse(Moto moto)
    {
        var resource = new ResourceResponse<Moto>(moto);
        var version = GetCurrentApiVersion();
        AddLink(resource.Links, Url.Action(nameof(GetMoto), new { version, id = moto.IdMoto }), "self", "GET");
        AddLink(resource.Links, Url.Action(nameof(UpdateMoto), new { version, id = moto.IdMoto }), "update", "PUT");
        AddLink(resource.Links, Url.Action(nameof(DeleteMoto), new { version, id = moto.IdMoto }), "delete", "DELETE");
        return resource;
    }


    private static void AddLink(ICollection<Link> links, string? href, string rel, string method)
    {
        if (!string.IsNullOrWhiteSpace(href))
        {
            links.Add(new Link
            {
                Href = href,
                Rel = rel,
                Method = method
            });
        }
    }

    private string GetCurrentApiVersion()
    {
        return HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";
    }
}
