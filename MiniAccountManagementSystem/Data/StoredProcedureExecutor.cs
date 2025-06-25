using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MiniAccountManagementSystem.Data
{
    public class StoredProcedureExecutor
    {
        private readonly ApplicationDbContext _context;

        public StoredProcedureExecutor(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> ExecuteNonQueryAsync(string spName, params SqlParameter[] parameters)
        {
            using var conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<DataTable> ExecuteDataTableAsync(string spName, params SqlParameter[] parameters)
        {
            using var conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            using var reader = await cmd.ExecuteReaderAsync();
            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }
    }
}
