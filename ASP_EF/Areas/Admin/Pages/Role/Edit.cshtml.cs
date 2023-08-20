using ASP_EF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ASP_EF.Areas.Admin.Pages.Role
{
    [Authorize(Policy = "AllowEditRole")]
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base(roleManager, myBlogContext)
        {

        }

        public class InputModel
        {
            [DisplayName("Tên của role")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự")]
            public string Name { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<IdentityRoleClaim<string>> Claims { get; set; }

        public IdentityRole Role { get; set; }

        public async Task<IActionResult> OnGetAsync(string roleid)
        {
            if(roleid == null)
                return  NotFound("Không tìm thấy role");

            Role = await _roleManager.FindByIdAsync(roleid);

            if(Role != null)
            {
                Input = new InputModel()
                {
                    Name = Role.Name
                };

                Claims = await context.RoleClaims.Where(rc => rc.RoleId == roleid).ToListAsync();

                return Page();
            }

            return NotFound("Không tìm thấy role");
        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if (roleid == null)
                return NotFound("Không tìm thấy role");

            Role = await _roleManager.FindByIdAsync(roleid);
            if(Role == null)
                return NotFound("Không tìm thấy role");

            Claims = await context.RoleClaims.Where(rc => rc.RoleId == roleid).ToListAsync();

            if (!ModelState.IsValid)
                return Page();

            string oldRole = Role.Name;
            Role.Name = Input.Name;
            var resultRole = await _roleManager.UpdateAsync(Role);

            if(resultRole.Succeeded)
            {
                StatusMessage = $"Đã đổi tên role '{oldRole}' thành '{Input.Name}'";
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
