using Dapper;
using Npgsql;
using MySecureBackend.WebApi.Interface;
using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public class SqlEnvironment2DRepository : IEnvironment2DRepository
    {
        private readonly string sqlConnectionString;

        public SqlEnvironment2DRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task InsertAsync(Environment2D environment2D)
        {
            using var conn = new NpgsqlConnection(sqlConnectionString);
            await conn.ExecuteAsync("INSERT INTO \"Environment2D\" (\"Id\", \"Name\", \"MaxHeight\", \"MaxLength\", \"UserId\") VALUES (@Id, @Name, @MaxHeight, @MaxLength, @UserId)", environment2D);
        }

        public async Task<Environment2D?> SelectAsync(Guid id)
        {
            using var conn = new NpgsqlConnection(sqlConnectionString);
            return await conn.QuerySingleOrDefaultAsync<Environment2D>("SELECT * FROM \"Environment2D\" WHERE \"Id\" = @Id", new { id });
        }

        public async Task<IEnumerable<Environment2D>> SelectAsync()
        {
            using var conn = new NpgsqlConnection(sqlConnectionString);
            return await conn.QueryAsync<Environment2D>("SELECT * FROM \"Environment2D\"");
        }

        public async Task UpdateAsync(Environment2D environment2D)
        {
            using var conn = new NpgsqlConnection(sqlConnectionString);
            await conn.ExecuteAsync("UPDATE \"Environment2D\" SET " +
                                     "\"Name\" = @Name, " +
                                     "\"MaxHeight\" = @MaxHeight, " +
                                     "\"MaxLength\" = @MaxLength, " +
                                     "\"UserId\" = @UserId " +
                                     "WHERE \"Id\" = @Id", environment2D);
        }

        public async Task DeleteAsync(Guid id)
        {
            using var conn = new NpgsqlConnection(sqlConnectionString);
            await conn.ExecuteAsync("DELETE FROM \"Environment2D\" WHERE \"Id\" = @Id", new { id });
        }

        public async Task<IEnumerable<Environment2D>> SelectByUserIdAsync(string userId)
        {
            using var conn = new NpgsqlConnection(sqlConnectionString);
            return await conn.QueryAsync<Environment2D>(
                "SELECT * FROM \"Environment2D\" WHERE \"UserId\" = @UserId",
                new { UserId = userId });
        }

        public async Task<Environment2D?> SelectByUserIdAndNameAsync(string userId, string name)
        {
            using var conn = new NpgsqlConnection(sqlConnectionString);
            return await conn.QuerySingleOrDefaultAsync<Environment2D>(
                "SELECT * FROM \"Environment2D\" WHERE \"UserId\" = @UserId AND \"Name\" = @Name",
                new { UserId = userId, Name = name });
        }
    }
}
