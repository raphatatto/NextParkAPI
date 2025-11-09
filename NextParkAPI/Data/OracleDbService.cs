using System.Data;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace NextParkAPI.Data
{
    public class OracleDbService
    {
        private readonly string _connStr;
        public OracleDbService(IConfiguration cfg)
            => _connStr = cfg.GetConnectionString("OracleDb")!; // usa sua key "OracleDb"

        public async Task<int> ExecProcAsync(string plsql, params OracleParameter[] parameters)
        {
            await using var conn = new OracleConnection(_connStr);
            await conn.OpenAsync();
            await using var cmd = new OracleCommand(plsql, conn) { CommandType = CommandType.Text };
            cmd.Parameters.AddRange(parameters);
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<string?> ExecFunctionScalarClobAsync(string sql, params OracleParameter[] parameters)
        {
            await using var conn = new OracleConnection(_connStr);
            await conn.OpenAsync();
            await using var cmd = new OracleCommand(sql, conn) { CommandType = CommandType.Text };
            cmd.Parameters.AddRange(parameters);
            var r = await cmd.ExecuteScalarAsync();
            return r?.ToString();
        }
    }
}
