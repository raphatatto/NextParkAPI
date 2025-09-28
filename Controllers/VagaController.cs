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
    public class VagaController : ControllerBase
    {
        private readonly NextParkContext _context;

        public VagaController(NextParkContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<Vaga>>> GetVagas([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Os parâmetros de paginação devem ser maiores que zero.");
            }

            var query = _context.Vagas.AsNoTracking().OrderBy(v => v.IdVaga);
            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var response = new PagedResponse<Vaga>(items, totalCount, pageNumber, pageSize);
            AddCollectionLinks(response, pageNumber, pageSize);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceResponse<Vaga>>> GetVaga(int id)
        {
            var vaga = await _context.Vagas.AsNoTracking().FirstOrDefaultAsync(v => v.IdVaga == id);
            if (vaga == null) return NotFound();
            var response = CreateResourceResponse(vaga);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ResourceResponse<Vaga>>> CreateVaga(Vaga vaga)
        {
            _context.Vagas.Add(vaga);
            await _context.SaveChangesAsync();
            var response = CreateResourceResponse(vaga);
            return CreatedAtAction(nameof(GetVaga), new { id = vaga.IdVaga }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVaga(int id, Vaga vaga)
        {
            if (id != vaga.IdVaga) return BadRequest();
            var exists = await _context.Vagas.AnyAsync(v => v.IdVaga == id);
            if (!exists) return NotFound();
            _context.Entry(vaga).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVaga(int id)
        {
            var vaga = await _context.Vagas.FindAsync(id);
            if (vaga == null) return NotFound();
            _context.Vagas.Remove(vaga);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private void AddCollectionLinks(PagedResponse<Vaga> response, int pageNumber, int pageSize)
        {
            AddLink(response.Links, Url.Action(nameof(GetVagas), new { pageNumber, pageSize }), "self", "GET");
            if (pageNumber > 1)
            {
                AddLink(response.Links, Url.Action(nameof(GetVagas), new { pageNumber = pageNumber - 1, pageSize }), "previous", "GET");
            }

            if (pageNumber < response.TotalPages)
            {
                AddLink(response.Links, Url.Action(nameof(GetVagas), new { pageNumber = pageNumber + 1, pageSize }), "next", "GET");
            }

            AddLink(response.Links, Url.Action(nameof(CreateVaga)), "create", "POST");
        }

        private ResourceResponse<Vaga> CreateResourceResponse(Vaga vaga)
        {
            var resource = new ResourceResponse<Vaga>(vaga);
            AddLink(resource.Links, Url.Action(nameof(GetVaga), new { id = vaga.IdVaga }), "self", "GET");
            AddLink(resource.Links, Url.Action(nameof(UpdateVaga), new { id = vaga.IdVaga }), "update", "PUT");
            AddLink(resource.Links, Url.Action(nameof(DeleteVaga), new { id = vaga.IdVaga }), "delete", "DELETE");
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
