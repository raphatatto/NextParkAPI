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

namespace NextParkAPI.Controllers;


[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class MotoController : ControllerBase
{
    private readonly NextParkContext _context;

    public MotoController(NextParkContext context)
    {
        _context = context;
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
    [SwaggerOperation(
        Summary = "Cadastrar moto",
        Description = "Registra uma nova moto no pátio e retorna o recurso criado." )]
    [ProducesResponseType(typeof(ResourceResponse<Moto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResourceResponse<Moto>>> CreateMoto(Moto moto)
    {
        _context.Motos.Add(moto);
        await _context.SaveChangesAsync();
        var response = CreateResourceResponse(moto);
        return CreatedAtAction(
            nameof(GetMoto),
            new { version = GetCurrentApiVersion(), id = moto.IdMoto },
            response);
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
