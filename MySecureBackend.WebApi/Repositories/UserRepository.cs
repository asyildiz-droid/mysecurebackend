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

            _connectionString = connectionString;

        }
        public async Task<User?> GetByUserName(string userName)

        {

            using var conn = new NpgsqlConnection(_connectionString);


            return await conn.QueryFirstOrDefaultAsync<User>(

                "SELECT * FROM \"User\" WHERE \"UserName\" = @UserName",

                new { UserName = userName });

        }
        public async Task Create(User user)

        {

            using var conn = new NpgsqlConnection(_connectionString);


            await conn.ExecuteAsync(

                "INSERT INTO \"User\" (\"UserName\", \"PasswordHash\") VALUES (@UserName, @PasswordHash)",

                user);

        }
    }
}
