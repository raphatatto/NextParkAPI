using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IReadOnlyCollection<IPrimaryKeyGenerator> _primaryKeyGenerators;
        private readonly string? _providerName;

        public AuthController(NextParkContext context, IEnumerable<IPrimaryKeyGenerator> primaryKeyGenerators)
        {
            _context = context;
            _primaryKeyGenerators = primaryKeyGenerators.ToArray();
            _providerName = _context.Database.ProviderName;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var emailAlreadyUsed = await _context.Usuarios
                .AsNoTracking()
                .AnyAsync(u => u.NrEmail == request.Email);

            if (emailAlreadyUsed)
            {
                return Conflict(new { message = "E-mail já cadastrado." });
            }

            var primaryKeyGenerator = ResolvePrimaryKeyGenerator();

            var usuario = new Usuario
            {
                NrEmail = request.Email
            };

            var usuarioId = await primaryKeyGenerator.GenerateAsync(
                _context,
                "TB_NEXTPARK_USUARIO",
                "ID_USUARIO",
                "SEQ_TB_NEXTPARK_USUARIO",
                "TB_NEXTPARK_USUARIO_SEQ",
                "SEQ_NEXTPARK_USUARIO",
                "SEQ_USUARIO");

            if (usuarioId.HasValue)
            {
                usuario.IdUsuario = usuarioId.Value;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                var loginId = await primaryKeyGenerator.GenerateAsync(
                    _context,
                    "TB_NEXTPARK_LOGIN",
                    "ID_LOGIN",
                    "SEQ_TB_NEXTPARK_LOGIN",
                    "TB_NEXTPARK_LOGIN_SEQ",
                    "SEQ_NEXTPARK_LOGIN",
                    "SEQ_LOGIN");

                var login = new Login
                {
                    IdUsuario = usuario.IdUsuario,
                    NrEmail = request.Email,
                    DsSenha = PasswordHasher.HashPassword(request.Password)
                };

                if (loginId.HasValue)
                {
                    login.IdLogin = loginId.Value;
                }

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

        private IPrimaryKeyGenerator ResolvePrimaryKeyGenerator()
        {
            var generator = _primaryKeyGenerators
                .FirstOrDefault(g => g.CanGenerate(_providerName));

            if (generator is null)
            {
                throw new InvalidOperationException($"Nenhum gerador de chave primária configurado para o provedor '{_providerName}'.");
            }

            return generator;
        }
    }
}
