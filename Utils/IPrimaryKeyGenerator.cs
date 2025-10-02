using System.Threading.Tasks;
using NextParkAPI.Data;

namespace NextParkAPI.Utils
{
    public interface IPrimaryKeyGenerator
    {
        bool CanGenerate(string? providerName);

        Task<int?> GenerateAsync(
            NextParkContext context,
            string tableName,
            string columnName,
            params string[] sequenceCandidates);
    }
}
