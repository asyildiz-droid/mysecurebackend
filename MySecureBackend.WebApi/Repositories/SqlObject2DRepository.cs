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
            await conn.ExecuteAsync("INSERT INTO \"Object2D\" (\"Id\", \"PrefabId\", \"PositionX\", \"PositionY\", \"ScaleX\", \"ScaleY\", \"RotationZ\", \"SortingLayer\") VALUES (@Id, @PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer)", object2D);
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
            await conn.ExecuteAsync("UPDATE \"Object2D\" SET " +
                                     "\"PrefabId\" = @PrefabId, " +
                                     "\"PositionX\" = @PositionX, " +
                                     "\"PositionY\" = @PositionY, " +
                                     "\"ScaleX\" = @ScaleX, " +
                                     "\"ScaleY\" = @ScaleY, " +
                                     "\"RotationZ\" = @RotationZ, " +
                                     "\"SortingLayer\" = @SortingLayer " +
                                     "WHERE \"Id\" = @Id", object2D);
        }

        public async Task DeleteAsync(Guid id)
        {
            using var conn = new NpgsqlConnection(sqlConnectionString);
            await conn.ExecuteAsync("DELETE FROM \"Object2D\" WHERE \"Id\" = @Id", new { id });
        }
    }
}
