// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ASP_EF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASP_EF.Areas.Admin.Pages.User
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyBlogContext _context;

        public AddRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            MyBlogContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }


        [TempData]
        public string StatusMessage { get; set; }

        public AppUser User { get; set; }

        [BindProperty]
        [DisplayName("Các role gán cho user")]
        public string[] RoleNames { get; set; }

        public SelectList AllRoles { get; set; }

        public List<IdentityRoleClaim<string>> ClaimInRole { get; set; }
        public List<IdentityUserClaim<string>> ClaimInUser { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Không có User");
            }

            User = await _userManager.FindByIdAsync(id);

            if (User == null)
            {
                return NotFound($"Không tìm thấy User với ID = '{id}'.");
            }

            RoleNames = (await _userManager.GetRolesAsync(User)).ToArray();

            List<string> listRoleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            AllRoles = new SelectList(listRoleNames);

            await GetClaims(id);

            return Page();
        }

        public async Task GetClaims(string id)
        {
            // List Role này là của User
            var listRoles = from r in _context.Roles
                            join ur in _context.UserRoles
                            on r.Id equals ur.RoleId
                            where ur.UserId == id
                            select r;

            var _claimInRole = from c in _context.RoleClaims
                               join r in listRoles on c.RoleId equals r.Id
                               select c;

            ClaimInRole = await _claimInRole.ToListAsync();

            ClaimInUser = await (from r in _context.UserClaims
                                 where r.UserId == id
                                 select r).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Không có User");
            }

            User = await _userManager.FindByIdAsync(id);

            if (User == null)
            {
                return NotFound($"Không tìm thấy User với ID = '{id}'.");
            }

            await GetClaims(id);

            // RoleNames
            var oldRoleNames = (await _userManager.GetRolesAsync(User)).ToArray();

            var deleteRole = oldRoleNames.Where(r => !RoleNames.Contains(r));
            var addRole = RoleNames.Where(r => !oldRoleNames.Contains(r));

            List<string> listRoleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            AllRoles = new SelectList(listRoleNames);

            var resultDelete = await _userManager.RemoveFromRolesAsync(User, deleteRole);
            if(!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            var resultAdd = await _userManager.AddToRolesAsync(User, addRole);
            if (!resultAdd.Succeeded)
            {
                resultAdd.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            StatusMessage = $"Đã cập nhật role cho: {User.UserName}";

            return RedirectToPage("./Index");
        }

    }
}
