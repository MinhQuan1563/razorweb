using ASP_EF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ASP_EF.Areas.Admin.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public class UserAndRole : AppUser
        {
            public string RoleNames { get; set; }
        }
        public List<UserAndRole> Users { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        // Paging
        public const int ITEMS_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int TotalUsers { get; set; }

        public async Task OnGetAsync()
        {
            // Users = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
            var qr = _userManager.Users.OrderBy(u => u.UserName);

            TotalUsers = await qr.CountAsync();
            TotalPage = (int)Math.Ceiling((double)TotalUsers / ITEMS_PER_PAGE);

            if (CurrentPage < 1)
                CurrentPage = 1;
            if (CurrentPage > TotalPage)
                CurrentPage = TotalPage;

            var qr1 = qr.Skip((CurrentPage - 1) * ITEMS_PER_PAGE)
                        .Take(ITEMS_PER_PAGE)
                        .Select(u => new UserAndRole()
                        {
                            Id = u.Id,
                            UserName = u.UserName
                        });

            Users = await qr1.ToListAsync();

            foreach(var user in Users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(", ", roles);
            }
        }

        public void OnPost() => RedirectToPage();
    }
}
