using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Data;
using MiniAccountManagementSystem.Models;
using System.Data;

namespace MiniAccountManagementSystem.Services
{
    public class AccountService
    {
        private readonly StoredProcedureExecutor _spExecutor;

        public AccountService(StoredProcedureExecutor spExecutor)
        {
            _spExecutor = spExecutor;
        }

        public async Task<int> CreateAccount(string name, int? parentId, string accountType)
        {
            var accountIdParam = new SqlParameter("@AccountId", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
            new SqlParameter("@Operation", "CREATE"),
            accountIdParam,
            new SqlParameter("@AccountName", name),
            new SqlParameter("@ParentAccountId", (object?)parentId ?? DBNull.Value),
            new SqlParameter("@AccountType", accountType)
        };
            await _spExecutor.ExecuteNonQueryAsync("sp_ManageChartOfAccounts", parameters);
            return (int)accountIdParam.Value;
        }

        public async Task UpdateAccount(int accountId, string name, int? parentId, string accountType)
        {
            var parameters = new[]
            {
            new SqlParameter("@Operation", "UPDATE"),
            new SqlParameter("@AccountId", accountId),
            new SqlParameter("@AccountName", name),
            new SqlParameter("@ParentAccountId", (object?)parentId ?? DBNull.Value),
            new SqlParameter("@AccountType", accountType)
        };
            await _spExecutor.ExecuteNonQueryAsync("sp_ManageChartOfAccounts", parameters);
        }

        public async Task DeleteAccount(int accountId)
        {
            var parameters = new[]
            {
            new SqlParameter("@Operation", "DELETE"),
            new SqlParameter("@AccountId", accountId)
        };
            await _spExecutor.ExecuteNonQueryAsync("sp_ManageChartOfAccounts", parameters);
        }

        public async Task<List<ChartOfAccount>> GetAllAccounts()
        {
            var dt = await _spExecutor.ExecuteDataTableAsync("sp_GetChartOfAccounts"); // Call the stored procedure

            var list = new List<ChartOfAccount>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ChartOfAccount
                {
                    AccountId = Convert.ToInt32(row["AccountId"]),
                    AccountName = row["AccountName"].ToString(),
                    ParentAccountId = row["ParentAccountId"] == DBNull.Value ? null : (int?)row["ParentAccountId"],
                    AccountType = row["AccountType"].ToString()
                });
            }
            return list;
        }

        public List<ChartOfAccount> BuildAccountTree(List<ChartOfAccount> flatList)
        {
            var lookup = flatList.ToLookup(a => a.ParentAccountId);
            foreach (var account in flatList)
            {
                account.Children = lookup[account.AccountId].ToList();
            }
            return lookup[null].ToList();
        }
    }
}
