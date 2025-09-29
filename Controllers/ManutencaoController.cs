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

/// <summary>
/// Administra os registros de manutenção realizados nas motos do pátio.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ManutencaoController : ControllerBase
{
    private readonly NextParkContext _context;

    public ManutencaoController(NextParkContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Recupera uma lista paginada de manutenções registradas.
    /// </summary>
    /// <param name="pageNumber">Número da página desejada (inicia em 1).</param>
    /// <param name="pageSize">Quantidade de registros por página.</param>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar manutenções",
        Description = "Retorna uma página de manutenções registradas ordenadas pelo identificador." )]
    [ProducesResponseType(typeof(PagedResponse<Manutencao>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<Manutencao>>> GetManutencoes([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return BadRequest("Os parâmetros de paginação devem ser maiores que zero.");
        }

        var query = _context.Manutencoes.AsNoTracking().OrderBy(m => m.IdManutencao);
        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        var response = new PagedResponse<Manutencao>(items, totalCount, pageNumber, pageSize);
        AddCollectionLinks(response, pageNumber, pageSize);

        return Ok(response);
    }

    /// <summary>
    /// Obtém uma manutenção específica pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da manutenção.</param>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Obter manutenção",
        Description = "Busca uma manutenção pelo identificador e retorna seus dados com enlaces HATEOAS." )]
    [ProducesResponseType(typeof(ResourceResponse<Manutencao>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResourceResponse<Manutencao>>> GetManutencao(int id)
    {
        var manutencao = await _context.Manutencoes.AsNoTracking().FirstOrDefaultAsync(m => m.IdManutencao == id);
        if (manutencao == null) return NotFound();
        var response = CreateResourceResponse(manutencao);
        return Ok(response);
    }

    /// <summary>
    /// Registra uma nova manutenção para uma moto.
    /// </summary>
    /// <param name="manutencao">Dados da manutenção a ser criada.</param>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastrar manutenção",
        Description = "Registra uma nova manutenção vinculada a uma moto existente." )]
    [ProducesResponseType(typeof(ResourceResponse<Manutencao>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResourceResponse<Manutencao>>> CreateManutencao(Manutencao manutencao)
    {
        var moto = await _context.Motos.FindAsync(manutencao.IdMoto);
        if (moto is null)
        {
            return BadRequest("A moto informada não existe.");
        }

        _context.Manutencoes.Add(manutencao);
        await _context.SaveChangesAsync();
        var response = CreateResourceResponse(manutencao);
        return CreatedAtAction(nameof(GetManutencao), new { id = manutencao.IdManutencao }, response);
    }

    /// <summary>
    /// Atualiza os dados de uma manutenção existente.
    /// </summary>
    /// <param name="id">Identificador da manutenção.</param>
    /// <param name="manutencao">Dados atualizados da manutenção.</param>
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualizar manutenção",
        Description = "Atualiza completamente os dados de uma manutenção existente." )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateManutencao(int id, Manutencao manutencao)
    {
        if (id != manutencao.IdManutencao) return BadRequest();

        var exists = await _context.Manutencoes.AnyAsync(m => m.IdManutencao == id);
        if (!exists) return NotFound();

        var moto = await _context.Motos.FindAsync(manutencao.IdMoto);
        if (moto is null)
        {
            return BadRequest("A moto informada não existe.");
        }

        _context.Entry(manutencao).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Remove um registro de manutenção.
    /// </summary>
    /// <param name="id">Identificador da manutenção.</param>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Excluir manutenção",
        Description = "Remove um registro de manutenção existente." )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteManutencao(int id)
    {
        var manutencao = await _context.Manutencoes.FindAsync(id);
        if (manutencao == null) return NotFound();
        _context.Manutencoes.Remove(manutencao);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Adiciona enlaces HATEOAS de navegação para o recurso de manutenções.
    /// </summary>
    private void AddCollectionLinks(PagedResponse<Manutencao> response, int pageNumber, int pageSize)
    {
        AddLink(response.Links, Url.Action(nameof(GetManutencoes), new { pageNumber, pageSize }), "self", "GET");
        if (pageNumber > 1)
        {
            AddLink(response.Links, Url.Action(nameof(GetManutencoes), new { pageNumber = pageNumber - 1, pageSize }), "previous", "GET");
        }

        if (pageNumber < response.TotalPages)
        {
            AddLink(response.Links, Url.Action(nameof(GetManutencoes), new { pageNumber = pageNumber + 1, pageSize }), "next", "GET");
        }

        AddLink(response.Links, Url.Action(nameof(CreateManutencao)), "create", "POST");
    }

    /// <summary>
    /// Cria o envelope de resposta de uma manutenção com enlaces HATEOAS.
    /// </summary>
    private ResourceResponse<Manutencao> CreateResourceResponse(Manutencao manutencao)
    {
        var resource = new ResourceResponse<Manutencao>(manutencao);
        AddLink(resource.Links, Url.Action(nameof(GetManutencao), new { id = manutencao.IdManutencao }), "self", "GET");
        AddLink(resource.Links, Url.Action(nameof(UpdateManutencao), new { id = manutencao.IdManutencao }), "update", "PUT");
        AddLink(resource.Links, Url.Action(nameof(DeleteManutencao), new { id = manutencao.IdManutencao }), "delete", "DELETE");
        return resource;
    }

    /// <summary>
    /// Registra um enlace HATEOAS caso a URL informada seja válida.
    /// </summary>
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
}
