using Dapper;
using MySecureBackend.WebApi.Interface;
using MySecureBackend.WebApi.Models;
using Npgsql;

namespace MySecureBackend.WebApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            // Fail-safe: als de connectionstring stiekem leeg wordt doorgegeven door Program.cs, crasht hij direct zichtbaar.
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "De database verbindingsstring is LEEG!");

            _connectionString = connectionString;
        }

        public async Task<User?> GetByUserName(string userName)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            // Haal de gebruiker op. Dapper map de "Id" (int), "UserName" en "PasswordHash" automatisch naar je User object.
            return await conn.QuerySingleOrDefaultAsync<User>(
                "SELECT \"Id\", \"UserName\", \"PasswordHash\" FROM \"User\" WHERE \"UserName\" = @UserName",
                new { UserName = userName });
        }

        public async Task Create(User user)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);

                // We dwingen expliciete parameters af voor de veiligheid
                var parameters = new DynamicParameters();
                parameters.Add("UserName", user.UserName);
                parameters.Add("PasswordHash", user.PasswordHash);

                await conn.ExecuteAsync(
                    "INSERT INTO \"User\" (\"UserName\", \"PasswordHash\") VALUES (@UserName, @PasswordHash)",
                    parameters);
            }
            catch (PostgresException ex)
            {
                Console.WriteLine($"[!! ERROR !!] SQL Fout bij aanmaken User: {ex.MessageText} (Detail: {ex.Detail})");
                throw new Exception($"Postgres Error in Create User: {ex.MessageText}");
            }
        }
    }
}
