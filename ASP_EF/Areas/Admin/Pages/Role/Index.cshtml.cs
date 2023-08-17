using ASP_EF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ASP_EF.Areas.Admin.Pages.Role
{
    [Authorize(Roles = "Admin,Editer")]
    [Authorize(Roles = "Vip")]
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base(roleManager, myBlogContext)
        {

        }

        public List<IdentityRole> ListRole { get; set; }

        public async Task OnGet()
        {
            ListRole = await _roleManager.Roles.OrderBy(role => role.Name).ToListAsync();
        }

        public void OnPost() => RedirectToPage();
    }
}
