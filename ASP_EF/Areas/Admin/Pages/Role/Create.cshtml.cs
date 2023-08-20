using ASP_EF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ASP_EF.Areas.Admin.Pages.Role
{
    public class CreateModel : RolePageModel
    {
        public CreateModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base(roleManager, myBlogContext)
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

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            var newRole = new IdentityRole(Input.Name);
            var resultRole = await _roleManager.CreateAsync(newRole);

            if(resultRole.Succeeded)
            {
                StatusMessage = $"Bạn vừa tạo role mới: {Input.Name}";
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
