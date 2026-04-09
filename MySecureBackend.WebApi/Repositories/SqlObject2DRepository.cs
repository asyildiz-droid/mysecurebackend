using Dapper;
using Npgsql;
using MySecureBackend.WebApi.Interface;
using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public class SqlObject2DRepository : IObject2DRepository
    {
        private readonly string sqlConnectionString;

        public SqlObject2DRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task InsertAsync(Object2D object2D)
        {
            using var conn = new NpgsqlConnection(sqlConnectionString);

            // Geforceerde Casts toegevoegd zodat DBeaver/Postgres nooit meer zeurt over "operator does not exist"
            await conn.ExecuteAsync(
                "INSERT INTO \"Object2D\" (\"Id\", \"PrefabId\", \"PositionX\", \"PositionY\", \"ScaleX\", \"ScaleY\", \"RotationZ\", \"SortingLayer\") " +
                "VALUES (@Id, @PrefabId, CAST(@PositionX AS REAL), CAST(@PositionY AS REAL), CAST(@ScaleX AS REAL), CAST(@ScaleY AS REAL), CAST(@RotationZ AS REAL), CAST(@SortingLayer AS INTEGER))",
                object2D);
        }

        public async Task<Object2D?> SelectAsync(Guid id)
        {
            using var conn = new NpgsqlConnection(sqlConnectionString);
            return await conn.QuerySingleOrDefaultAsync<Object2D>("SELECT * FROM \"Object2D\" WHERE \"Id\" = @Id", new { id });
        }

        public async Task<IEnumerable<Object2D>> SelectAsync()
        {
            using var conn = new NpgsqlConnection(sqlConnectionString);
            return await conn.QueryAsync<Object2D>("SELECT * FROM \"Object2D\"");
        }

        public async Task UpdateAsync(Object2D object2D)
        {
            using var conn = new NpgsqlConnection(sqlConnectionString);

            await conn.ExecuteAsync(
                "UPDATE \"Object2D\" SET " +
                "\"PrefabId\" = @PrefabId, " +
                "\"PositionX\" = CAST(@PositionX AS REAL), " +
                "\"PositionY\" = CAST(@PositionY AS REAL), " +
                "\"ScaleX\" = CAST(@ScaleX AS REAL), " +
                "\"ScaleY\" = CAST(@ScaleY AS REAL), " +
                "\"RotationZ\" = CAST(@RotationZ AS REAL), " +
                "\"SortingLayer\" = CAST(@SortingLayer AS INTEGER) " +
                "WHERE \"Id\" = @Id",
                object2D);
        }

        public async Task DeleteAsync(Guid id)
        {
            using var conn = new NpgsqlConnection(sqlConnectionString);
            await conn.ExecuteAsync("DELETE FROM \"Object2D\" WHERE \"Id\" = @Id", new { id });
        }
    }
}
