using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextParkAPI.Data;
using NextParkAPI.Models;
using NextParkAPI.Models.Requests;
using NextParkAPI.Utils;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Threading.Tasks;

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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var emailAlreadyUsed = await EmailExistsAsync(request.Email);

            if (emailAlreadyUsed)
            {
                return Conflict(new { message = "E-mail já cadastrado." });
            }

            var connectionString = GetConnectionStringOrThrow();

            var usuarioId = await OraclePrimaryKeyGenerator.GenerateAsync(
                connectionString,
                "TB_NEXTPARK_USUARIO",
                "ID_USUARIO",
                "SEQ_TB_NEXTPARK_USUARIO",
                "TB_NEXTPARK_USUARIO_SEQ",
                "SEQ_NEXTPARK_USUARIO",
                "SEQ_USUARIO");

            var usuario = new Usuario
            {
                IdUsuario = usuarioId,
                NrEmail = request.Email
            };

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                var loginId = await OraclePrimaryKeyGenerator.GenerateAsync(
                    connectionString,
                    "TB_NEXTPARK_LOGIN",
                    "ID_LOGIN",
                    "SEQ_TB_NEXTPARK_LOGIN",
                    "TB_NEXTPARK_LOGIN_SEQ",
                    "SEQ_NEXTPARK_LOGIN",
                    "SEQ_LOGIN");

                var login = new Login
                {
                    IdLogin = loginId,
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

        private string GetConnectionStringOrThrow()
        {
            var connectionString = _context.Database.GetConnectionString();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            return connectionString;
        }

        private async Task<bool> EmailExistsAsync(string email)
        {
            var connectionString = GetConnectionStringOrThrow();

            await using var connection = new OracleConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.BindByName = true;
            command.CommandText = "SELECT COUNT(1) FROM TB_NEXTPARK_USUARIO WHERE NR_EMAIL = :email";

            var emailParameter = new OracleParameter("email", OracleDbType.Varchar2, email, ParameterDirection.Input);
            command.Parameters.Add(emailParameter);

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }
    }
}
