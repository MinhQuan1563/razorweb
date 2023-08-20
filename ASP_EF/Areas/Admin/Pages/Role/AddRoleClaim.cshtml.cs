using ASP_EF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ASP_EF.Areas.Admin.Pages.Role
{
    public class AddRoleClaimModel : RolePageModel
    {
        public AddRoleClaimModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base(roleManager, myBlogContext)
        {

        }

        public class InputModel
        {
            [DisplayName("Kiểu (tên) claim")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự")]
            public string ClaimType { get; set; }

            [DisplayName("Giá trị của claim")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự")]
            public string ClaimValue { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IdentityRole Role { get; set; }

        public async Task<IActionResult> OnGet(string roleid)
        {
            Role = await _roleManager.FindByIdAsync(roleid);
            if(Role == null)
                return NotFound($"Không tìm thấy role có id = {roleid}");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            Role = await _roleManager.FindByIdAsync(roleid);
            if (Role == null)
                return NotFound($"Không tìm thấy role có id = {roleid}");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if((await _roleManager.GetClaimsAsync(Role)).Any(claim => claim.Type == Input.ClaimType && claim.Value == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có trong Role");
                return Page();
            }

            var newClaim = new Claim(Input.ClaimType, Input.ClaimValue);
            var result = await _roleManager.AddClaimAsync(Role, newClaim);
            if(!result.Succeeded)
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            StatusMessage = "Vừa thêm đặc tính (claim) mới";

            return RedirectToPage("./Edit", new { roleid });
        }
    }
}
