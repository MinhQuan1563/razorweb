using ASP_EF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ASP_EF.Areas.Admin.Pages.User
{
    public class EditUserRoleClaimModel : PageModel
    {
        private readonly MyBlogContext _context;
        private readonly UserManager<AppUser> _userManager;
        public EditUserRoleClaimModel(MyBlogContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public NotFoundObjectResult OnGet() => NotFound("Không được truy cập");

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

        public AppUser User { get; set; }

        public async Task<IActionResult> OnGetAddClaimAsync(string userid)
        {
            User = await _userManager.FindByIdAsync(userid);
            if(User == null)
                return NotFound("Không tìm thấy user");

            return Page();
        }

        public IdentityUserClaim<string> UserClaim { get; set; }

        public async Task<IActionResult> OnPostAddClaimAsync(string userid)
        {
            User = await _userManager.FindByIdAsync(userid);
            if (User == null)
                return NotFound("Không tìm thấy user");

            if(!ModelState.IsValid) return Page();

            // Danh sách các claim của User
            var claims = _context.UserClaims.Where(c => c.UserId == User.Id);
            
            if(claims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Đặc tính (claim) này đã có");
                return Page();
            }

            await _userManager.AddClaimAsync(User, new Claim(Input.ClaimType, Input.ClaimValue));
            StatusMessage = "Đã thêm đặc tính (claim) cho User";

            return RedirectToPage("./AddRole", new {id = User.Id});
        }

        public async Task<IActionResult> OnGetEditClaimAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy User");

            UserClaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            User = await _userManager.FindByIdAsync(UserClaim.UserId);
            if (User == null)
                return NotFound("Không tìm thấy user");

            Input = new()
            {
                ClaimType = UserClaim.ClaimType,
                ClaimValue = UserClaim.ClaimValue
            };

            return Page();
        }

        public async Task<IActionResult> OnPostEditClaimAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy User");

            UserClaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            User = await _userManager.FindByIdAsync(UserClaim.UserId);
            if (User == null)
                return NotFound("Không tìm thấy user");

            if(!ModelState.IsValid) return Page();

            if(_context.UserClaims.Any(c => c.UserId == User.Id
                && c.ClaimType == Input.ClaimType
                && c.ClaimValue == Input.ClaimValue
                && c.Id != claimid))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có");
            }

            UserClaim.ClaimType = Input.ClaimType;
            UserClaim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();
            StatusMessage = "Bạn vừa cập nhật claim";

            return RedirectToPage("./AddRole", new { id = User.Id });
        }

        public async Task<IActionResult> OnPostDeleteClaimAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy User");

            UserClaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            User = await _userManager.FindByIdAsync(UserClaim.UserId);
            if (User == null) return NotFound("Không tìm thấy user");

            await _userManager.RemoveClaimAsync(User, new Claim(UserClaim.ClaimType, UserClaim.ClaimValue));

            StatusMessage = "Bạn vừa xóa claim";

            return RedirectToPage("./AddRole", new { id = User.Id });
        }
    }
}
