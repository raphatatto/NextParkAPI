using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextParkAPI.Data;
using NextParkAPI.Models;
using NextParkAPI.Models.Requests;
using NextParkAPI.Utils;

namespace NextParkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly NextParkContext _context;

        public AuthController(NextParkContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registra um novo usuário na plataforma.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Usuarios.AnyAsync(u => u.NrEmail == request.Email))
            {
                return Conflict(new { message = "E-mail já cadastrado." });
            }

            var usuario = new Usuario
            {
                NrEmail = request.Email
            };

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                var login = new Login
                {
                    IdUsuario = usuario.IdUsuario,
                    NrEmail = request.Email,
                    DsSenha = PasswordHasher.HashPassword(request.Password)
                };

                _context.Logins.Add(login);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(Register), new
                {
                    message = "Usuário registrado com sucesso.",
                    usuarioId = usuario.IdUsuario,
                    email = usuario.NrEmail
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Realiza o login de um usuário existente.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var login = await _context.Logins
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.NrEmail == request.Email);

            if (login is null)
            {
                return Unauthorized(new { message = "E-mail ou senha inválidos." });
            }

            var passwordValid = PasswordHasher.VerifyPassword(request.Password, login.DsSenha);
            if (!passwordValid)
            {
                return Unauthorized(new { message = "E-mail ou senha inválidos." });
            }

            return Ok(new
            {
                message = "Login realizado com sucesso.",
                usuarioId = login.IdUsuario,
                email = login.NrEmail
            });
        }
    }
}
