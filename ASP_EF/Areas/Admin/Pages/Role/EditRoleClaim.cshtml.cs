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
    public class EditRoleClaimModel : RolePageModel
    {
        public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base(roleManager, myBlogContext)
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

        public IdentityRoleClaim<string> Claim { get; set; }

        public async Task<IActionResult> OnGet(int? claimid)
        {
            if(claimid == null)
                return NotFound("Không tìm thấy claim có id phù hợp");

            Claim = context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (Claim == null)
                return NotFound($"Không tìm thấy claim có id = {claimid}");

            Role = await _roleManager.FindByIdAsync(Claim.RoleId);
            if(Role == null)
                return NotFound($"Không tìm thấy role có id = {Claim.RoleId}");

            Input = new()
            {
                ClaimType = Claim.ClaimType,
                ClaimValue = Claim.ClaimValue
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? claimid )
        {
            if (claimid == null)
                return NotFound("Không tìm thấy claim có id phù hợp");

            Claim = context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (Claim == null)
                return NotFound($"Không tìm thấy claim có id = {claimid}");

            Role = await _roleManager.FindByIdAsync(Claim.RoleId);
            if (Role == null)
                return NotFound($"Không tìm thấy role có id = {Claim.RoleId}");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check trùng dữ liệu
            if(context.RoleClaims.Any(claim => claim.RoleId == Role.Id && claim.ClaimType == Input.ClaimType && claim.ClaimValue == Input.ClaimValue && claim.Id != Claim.Id))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có trong Role");
                return Page();
            }

            Claim.ClaimType = Input.ClaimType;
            Claim.ClaimValue = Input.ClaimValue;

            await context.SaveChangesAsync();

            StatusMessage = "Đã cập nhật thành công claim";

            return RedirectToPage("./Edit", new { roleid = Role.Id });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? claimid)
        {
            if (claimid == null)
                return NotFound("Không tìm thấy claim có id phù hợp");

            Claim = context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (Claim == null)
                return NotFound($"Không tìm thấy claim có id = {claimid}");

            Role = await _roleManager.FindByIdAsync(Claim.RoleId);
            if (Role == null)
                return NotFound($"Không tìm thấy role có id = {Claim.RoleId}");

            await _roleManager.RemoveClaimAsync(Role, new Claim(Claim.ClaimType, Claim.ClaimValue));

            StatusMessage = "Vừa xóa claim";

            return RedirectToPage("./Edit", new { roleid = Role.Id });
        }
    }
}
