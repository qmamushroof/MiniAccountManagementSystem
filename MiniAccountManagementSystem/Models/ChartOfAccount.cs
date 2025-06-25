namespace MiniAccountManagementSystem.Models
{
    public class ChartOfAccount
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public int? ParentAccountId { get; set; }
        public string AccountType { get; set; }
        public List<ChartOfAccount> Children { get; set; } = new();
    }

}
