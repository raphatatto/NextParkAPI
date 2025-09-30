using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace NextParkAPI.Utils
{
    public static class OraclePrimaryKeyGenerator
    {
        public static Task<int> GenerateAsync(
            string connectionString,
            string tableName,
            string columnName,
            params string[] sequenceCandidates) =>
            GenerateAsync(connectionString, tableName, columnName, (IReadOnlyCollection<string>)sequenceCandidates);

        private static async Task<int> GenerateAsync(
            string connectionString,
            string tableName,
            string columnName,
            IReadOnlyCollection<string> sequenceCandidates)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            await using var connection = new OracleConnection(connectionString);
            await connection.OpenAsync();

            return await GenerateWithConnectionAsync(connection, tableName, columnName, sequenceCandidates);
        }

        private static async Task<int> GenerateWithConnectionAsync(
            OracleConnection connection,
            string tableName,
            string columnName,
            IReadOnlyCollection<string> sequenceCandidates)
        {
            var candidates = await BuildSequenceCandidatesAsync(connection, tableName, sequenceCandidates);

            foreach (var sequenceName in candidates)
            {
                var nextValue = await TryGetNextSequenceValueAsync(connection, sequenceName);
                if (nextValue.HasValue)
                {
                    return nextValue.Value;
                }
            }

            return await GenerateFromTableAsync(connection, tableName, columnName);
        }

        private static async Task<List<string>> BuildSequenceCandidatesAsync(
            OracleConnection connection,
            string tableName,
            IReadOnlyCollection<string> sequenceCandidates)
        {
            var candidates = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            void AddCandidate(string? sequenceName)
            {
                if (string.IsNullOrWhiteSpace(sequenceName))
                {
                    return;
                }

                var normalized = sequenceName.Trim().ToUpperInvariant();
                if (seen.Add(normalized))
                {
                    candidates.Add(normalized);
                }
            }

            if (sequenceCandidates is { Count: > 0 })
            {
                foreach (var candidate in sequenceCandidates)
                {
                    AddCandidate(candidate);
                }
            }

            var normalizedTableName = tableName.Trim().ToUpperInvariant();
            AddCandidate($"{normalizedTableName}_SEQ");
            AddCandidate($"SEQ_{normalizedTableName}");

            string tableSuffix = normalizedTableName.StartsWith("TB_", StringComparison.Ordinal)
                ? normalizedTableName[3..]
                : normalizedTableName;

            AddCandidate($"{tableSuffix}_SEQ");
            AddCandidate($"SEQ_{tableSuffix}");
            AddCandidate($"SEQ_NEXTPARK_{tableSuffix}");
            AddCandidate($"NEXTPARK_{tableSuffix}_SEQ");

            await using (var command = connection.CreateCommand())
            {
                command.BindByName = true;
                command.CommandText = @"
                    SELECT SEQUENCE_NAME
                    FROM USER_SEQUENCES
                    WHERE SEQUENCE_NAME LIKE :tablePattern OR SEQUENCE_NAME LIKE :suffixPattern
                    ORDER BY SEQUENCE_NAME";

                command.Parameters.Add(new OracleParameter("tablePattern", OracleDbType.Varchar2, $"%{normalizedTableName}%", ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("suffixPattern", OracleDbType.Varchar2, $"%{tableSuffix}%", ParameterDirection.Input));

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    AddCandidate(reader.GetString(0));
                }
            }

            return candidates;
        }

        private static async Task<int?> TryGetNextSequenceValueAsync(OracleConnection connection, string sequenceName)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = $"SELECT {sequenceName}.NEXTVAL FROM DUAL";

            try
            {
                var result = await command.ExecuteScalarAsync();
                return result is null || result is DBNull ? null : Convert.ToInt32(result);
            }
            catch (OracleException ex) when (ex.Number is 2289 or 942)
            {
                return null;
            }
        }

        private static async Task<int> GenerateFromTableAsync(OracleConnection connection, string tableName, string columnName)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = $"SELECT NVL(MAX({columnName}), 0) + 1 FROM {tableName}";

            var result = await command.ExecuteScalarAsync();
            if (result is null || result is DBNull)
            {
                return 1;
            }

            return Convert.ToInt32(result);
        }
    }
}
