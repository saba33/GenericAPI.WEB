using System.Data.SqlClient;

namespace GenericAPI.Services.Helper
{
    public class AdoNetRepository<T> where T : class
    {

        private readonly string _connectionString;

        public AdoNetRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> InsertRecord(string tableName, T record)
        {
            var properties = typeof(T).GetProperties();

            var sql = $"INSERT INTO {tableName} (";

            foreach (var property in properties)
            {
                sql += $"{property.Name}, ";
            }

            sql = sql.TrimEnd(',', ' ');
            sql += ") VALUES (";

            foreach (var property in properties)
            {
                sql += $"@{property.Name}, ";
            }

            sql = sql.TrimEnd(',', ' ');
            sql += ")";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(sql, connection))
                {
                    foreach (var property in properties)
                    {
                        var parameter = new SqlParameter($"@{property.Name}", property.GetValue(record));
                        command.Parameters.Add(parameter);
                    }

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}

