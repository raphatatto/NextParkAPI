using System;
using System.Threading.Tasks;
using NextParkAPI.Data;

namespace NextParkAPI.Utils
{
    public class SqlServerPrimaryKeyGenerator : IPrimaryKeyGenerator
    {
        public bool CanGenerate(string? providerName) =>
            providerName?.IndexOf("SqlServer", StringComparison.OrdinalIgnoreCase) >= 0;

        public Task<int?> GenerateAsync(
            NextParkContext context,
            string tableName,
            string columnName,
            params string[] sequenceCandidates)
        {
            if (!CanGenerate(context.Database.ProviderName))
            {
                return Task.FromResult<int?>(null);
            }

            // SQL Server utiliza colunas IDENTITY para gerar as chaves automaticamente.
            return Task.FromResult<int?>(null);
        }
    }
}
