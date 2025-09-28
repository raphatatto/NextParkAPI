using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextParkAPI.Data;
using NextParkAPI.Models;
using NextParkAPI.Models.Responses;
namespace NextParkAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
public class MotoController : ControllerBase
{
    private readonly NextParkContext _context;

    public MotoController(NextParkContext context)
    {
        _context = context;
    }

    [HttpGet]
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
    public async Task<ActionResult<ResourceResponse<Moto>>> GetMoto(int id)
    {
        var moto = await _context.Motos.AsNoTracking().FirstOrDefaultAsync(m => m.IdMoto == id);
        if (moto == null) return NotFound();
        var response = CreateResourceResponse(moto);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ResourceResponse<Moto>>> CreateMoto(Moto moto)
    {
        _context.Motos.Add(moto);
        await _context.SaveChangesAsync();
        var response = CreateResourceResponse(moto);
        return CreatedAtAction(nameof(GetMoto), new { id = moto.IdMoto }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateMoto(int id, Moto moto)
    {
        if (id != moto.IdMoto) return BadRequest();
        var exists = await _context.Motos.AnyAsync(m => m.IdMoto == id);
        if (!exists) return NotFound();
        _context.Entry(moto).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
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
        AddLink(response.Links, Url.Action(nameof(GetMotos), new { pageNumber, pageSize }), "self", "GET");
        if (pageNumber > 1)
        {
            AddLink(response.Links, Url.Action(nameof(GetMotos), new { pageNumber = pageNumber - 1, pageSize }), "previous", "GET");
        }

        if (pageNumber < response.TotalPages)
        {
            AddLink(response.Links, Url.Action(nameof(GetMotos), new { pageNumber = pageNumber + 1, pageSize }), "next", "GET");
        }

        AddLink(response.Links, Url.Action(nameof(CreateMoto)), "create", "POST");
    }

    private ResourceResponse<Moto> CreateResourceResponse(Moto moto)
    {
        var resource = new ResourceResponse<Moto>(moto);
        AddLink(resource.Links, Url.Action(nameof(GetMoto), new { id = moto.IdMoto }), "self", "GET");
        AddLink(resource.Links, Url.Action(nameof(UpdateMoto), new { id = moto.IdMoto }), "update", "PUT");
        AddLink(resource.Links, Url.Action(nameof(DeleteMoto), new { id = moto.IdMoto }), "delete", "DELETE");
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
}
