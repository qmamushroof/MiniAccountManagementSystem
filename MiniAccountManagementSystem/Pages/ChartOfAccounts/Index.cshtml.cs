using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniAccountManagementSystem.Models;
using MiniAccountManagementSystem.Services;

namespace MiniAccountManagementSystem.Pages.ChartOfAccounts
{
    [Authorize(Roles = "Admin,Accountant,Viewer")]
    public class IndexModel : PageModel
    {
        private readonly AccountService _accountService;

        public List<ChartOfAccount> AccountTree { get; set; }

        public IndexModel(AccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task OnGet()
        {
            var flatList = await _accountService.GetAllAccounts();
            AccountTree = _accountService.BuildAccountTree(flatList);
        }
    }
}
