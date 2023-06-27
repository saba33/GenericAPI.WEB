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

        public decimal GetPlayerBalance(int playerId)
        {
            string connectionString = _connectionString;
            string query = "SELECT Balance FROM Players WHERE Id = @PlayerId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PlayerId", playerId);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToDecimal(result);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        public void UpdatePlayerBalance(int playerId, decimal newBalance)
        {
            string connectionString = _connectionString;
            string query = "UPDATE Players SET Balance = @NewBalance WHERE Id = @PlayerId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NewBalance", newBalance);
                    command.Parameters.AddWithValue("@PlayerId", playerId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        // Handle the case when the player is not found or the update fails
                        throw new Exception("Failed to update player balance.");
                    }
                }
            }
        }

        public bool CheckIfTransactionExists(long transactionId)
        {
            string connectionString = _connectionString;
            string query = "SELECT COUNT(*) FROM Transaction WHERE Id = @TransactionId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TransactionId", transactionId);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();

                    return count >= 0;
                }
            }
        }
    }
}

