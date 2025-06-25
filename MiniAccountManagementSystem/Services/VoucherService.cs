using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Data;
using MiniAccountManagementSystem.Models;
using System.Data;

namespace MiniAccountManagementSystem.Services
{
    public class VoucherService
    {
        private readonly StoredProcedureExecutor _spExecutor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VoucherService(StoredProcedureExecutor spExecutor, IHttpContextAccessor httpContextAccessor)
        {
            _spExecutor = spExecutor;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> SaveVoucher(Voucher voucher)
        {
            var voucherIdParam = new SqlParameter("@VoucherId", SqlDbType.Int)
            {
                Direction = ParameterDirection.InputOutput,
                Value = voucher.VoucherId == 0 ? DBNull.Value : (object)voucher.VoucherId
            };

            var entriesTable = new DataTable();
            entriesTable.Columns.Add("AccountId", typeof(int));
            entriesTable.Columns.Add("Debit", typeof(decimal));
            entriesTable.Columns.Add("Credit", typeof(decimal));
            foreach (var entry in voucher.Entries)
            {
                entriesTable.Rows.Add(entry.AccountId, entry.Debit, entry.Credit);
            }

            var parameters = new[]
            {
            voucherIdParam,
            new SqlParameter("@VoucherDate", voucher.VoucherDate),
            new SqlParameter("@ReferenceNo", voucher.ReferenceNo),
            new SqlParameter("@VoucherType", voucher.VoucherType),
            new SqlParameter("@CreatedBy", _httpContextAccessor.HttpContext.User.Identity.Name),
            new SqlParameter("@Entries", SqlDbType.Structured)
            {
                TypeName = "TVP_VoucherEntries",
                Value = entriesTable
            }
        };

            await _spExecutor.ExecuteNonQueryAsync("sp_SaveVoucher", parameters);

            return (int)voucherIdParam.Value;
        }
    }
}
