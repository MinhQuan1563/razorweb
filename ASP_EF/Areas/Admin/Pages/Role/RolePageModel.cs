using ASP_EF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_EF.Areas.Admin.Pages.Role
{
    public class RolePageModel : PageModel
    {
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly MyBlogContext context;

        [TempData]
        public string StatusMessage { get; set; }

        public RolePageModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext)
        {
            _roleManager = roleManager;
            context = myBlogContext;
        }
    }
}
