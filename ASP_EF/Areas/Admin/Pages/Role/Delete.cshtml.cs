using ASP_EF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ASP_EF.Areas.Admin.Pages.Role
{
    public class DeleteModel : RolePageModel
    {
        public DeleteModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base(roleManager, myBlogContext)
        {

        }

        public IdentityRole Role { get; set; }

        public async Task<IActionResult> OnGetAsync(string roleid)
        {
            if (roleid == null)
                return NotFound("Không tìm thấy role");

            Role = await _roleManager.FindByIdAsync(roleid);

            if (Role == null)
                return NotFound("Không tìm thấy role");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if (roleid == null)
                return NotFound("Không tìm thấy role");

            Role = await _roleManager.FindByIdAsync(roleid);
            if (Role == null)
                return NotFound("Không tìm thấy role");

            if (!ModelState.IsValid)
                return Page();

            var resultRole = await _roleManager.DeleteAsync(Role);

            if (resultRole.Succeeded)
            {
                StatusMessage = $"Bạn đã xóa role '{Role.Name}'";
                return RedirectToPage("./index");
            }
            else
            {
                resultRole.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }

            return Page();
        }
    }
}
