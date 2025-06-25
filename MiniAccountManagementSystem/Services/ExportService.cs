using ClosedXML.Excel;
using MiniAccountManagementSystem.Models;

namespace MiniAccountManagementSystem.Services
{
    public class ExportService
    {
        public byte[] ExportChartOfAccountsToExcel(List<ChartOfAccount> accounts)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("ChartOfAccounts");

            ws.Cell(1, 1).Value = "AccountId";
            ws.Cell(1, 2).Value = "AccountName";
            ws.Cell(1, 3).Value = "ParentAccountId";
            ws.Cell(1, 4).Value = "AccountType";

            int row = 2;
            void WriteAccount(ChartOfAccount acc, int level)
            {
                ws.Cell(row, 1).Value = acc.AccountId;
                ws.Cell(row, 2).Value = new string('-', level * 2) + acc.AccountName;
                ws.Cell(row, 3).Value = acc.ParentAccountId;
                ws.Cell(row, 4).Value = acc.AccountType;
                row++;

                foreach (var child in acc.Children)
                    WriteAccount(child, level + 1);
            }

            foreach (var acc in accounts)
            {
                WriteAccount(acc, 0);
            }

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }
    }

}
