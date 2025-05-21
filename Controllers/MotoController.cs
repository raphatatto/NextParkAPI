using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextParkAPI.Data;
using NextParkAPI.Models;
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
    public async Task<ActionResult<IEnumerable<Moto>>> GetMotos() =>
        Ok(await _context.Motos.ToListAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Moto>> GetMoto(int id)
    {
        var moto = await _context.Motos.FindAsync(id);
        if (moto == null) return NotFound();
        return Ok(moto);
    }

    [HttpPost]
    public async Task<ActionResult> CreateMoto(Moto moto)
    {
        _context.Motos.Add(moto);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMoto), new { id = moto.IdMoto }, moto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateMoto(int id, Moto moto)
    {
        if (id != moto.IdMoto) return BadRequest();
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
}
