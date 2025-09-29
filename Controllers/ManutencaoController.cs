using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextParkAPI.Data;
using NextParkAPI.Models;
using NextParkAPI.Models.Responses;

namespace NextParkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManutencaoController : ControllerBase
    {
        private readonly NextParkContext _context;

        public ManutencaoController(NextParkContext context)
        {
            _context = context;
        }

        [HttpGet]
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

        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceResponse<Manutencao>>> GetManutencao(int id)
        {
            var manutencao = await _context.Manutencoes.AsNoTracking().FirstOrDefaultAsync(m => m.IdManutencao == id);
            if (manutencao == null) return NotFound();
            var response = CreateResourceResponse(manutencao);
            return Ok(response);
        }

        [HttpPost]
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

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManutencao(int id)
        {
            var manutencao = await _context.Manutencoes.FindAsync(id);
            if (manutencao == null) return NotFound();
            _context.Manutencoes.Remove(manutencao);
            await _context.SaveChangesAsync();
            return NoContent();
        }

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

        private ResourceResponse<Manutencao> CreateResourceResponse(Manutencao manutencao)
        {
            var resource = new ResourceResponse<Manutencao>(manutencao);
            AddLink(resource.Links, Url.Action(nameof(GetManutencao), new { id = manutencao.IdManutencao }), "self", "GET");
            AddLink(resource.Links, Url.Action(nameof(UpdateManutencao), new { id = manutencao.IdManutencao }), "update", "PUT");
            AddLink(resource.Links, Url.Action(nameof(DeleteManutencao), new { id = manutencao.IdManutencao }), "delete", "DELETE");
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
}
