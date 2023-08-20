using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP_EF.Models;
using Microsoft.AspNetCore.Authorization;

namespace ASP_EF.Pages.Blog
{
    public class EditModel : PageModel
    {
        private readonly ASP_EF.Models.MyBlogContext _context;
        private readonly IAuthorizationService _authorizationService;

        public EditModel(ASP_EF.Models.MyBlogContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        [BindProperty]
        public Article Article { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.articles == null)
            {
                return Content("OnGet không tìm thấy bài viết (id == null)");
            }

            var article =  await _context.articles.FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return Content("OnGet không tìm thấy bài viết (id không tồn tại)");
            }
            Article = article;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Article).State = EntityState.Modified;

            try
            {
                // Kiểm tra quyền cập nhật
                var canupdate = await _authorizationService.AuthorizeAsync(this.User, Article, "CanUpdateArticle");
                if(canupdate.Succeeded)
                {
                    await _context.SaveChangesAsync();
                }
                else
                    return Content("Không được quyền cập nhật");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(Article.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ArticleExists(int id)
        {
          return (_context.articles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
