using ASP_EF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ASP_EF.Areas.Admin.Pages.Role
{
    //[Authorize(Policy = "AllowEditRole")]
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base(roleManager, myBlogContext)
        {

        }

        public class RoleModel : IdentityRole
        {
            public string[] Claims { get; set; }
        }

        public List<RoleModel> roles { get; set; }

        public async Task OnGet()
        {
            var r = await _roleManager.Roles.OrderBy(role => role.Name).ToListAsync();
            roles = new();

            foreach(var _r in r)
            {
                var claims = await _roleManager.GetClaimsAsync(_r);
                var claimsString = claims.Select(c => c.Type + " = " + c.Value);

                var rm = new RoleModel()
                {
                    Id = _r.Id,
                    Name = _r.Name,
                    Claims = claimsString.ToArray(),
                };
                roles.Add(rm);
            }
        }

        public void OnPost() => RedirectToPage();
    }
}
