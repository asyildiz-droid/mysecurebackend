using Dapper;
using Microsoft.Data.SqlClient;
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
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
               await sqlConnection.ExecuteAsync("INSERT INTO [Environment2D] (Id, Name, MaxHeight, MaxLength) VALUES (@Id, @Name, @MaxHeight, @MaxLength)", environment2D);
            }
        }

        public async Task<Environment2D?> SelectAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QuerySingleOrDefaultAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE Id = @Id", new { id });   
            }
        }

        public async Task<IEnumerable<Environment2D>> SelectAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<Environment2D>("SELECT * FROM [Environment2D]");
            }
        }

        public async Task UpdateAsync(Environment2D environment2D)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("UPDATE [Environment2D] SET " +
                                                 "Name = @Name, " +
                                                 "MaxHeight = @MaxHeight, " +
                                                 "MaxLength = @MaxLength " +
                                                 "WHERE Id = @Id", environment2D);

            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("DELETE FROM [Environment2D] WHERE Id = @Id", new { id });
            }
        }
    }
}
