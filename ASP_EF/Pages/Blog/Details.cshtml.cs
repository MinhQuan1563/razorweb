using ASP_EF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ASP_EF.Pages.Blog
{
    [Authorize(Policy = "InGenZ")] // Năm sinh từ 1997 -> 2012
    public class DetailsModel : PageModel
    {
        private readonly MyBlogContext _context;

        public DetailsModel(MyBlogContext context)
        {
            _context = context;
        }

      public Article Article { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.articles == null)
            {
                return NotFound();
            }

            var article = await _context.articles.FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            else 
            {
                Article = article;
            }
            return Page();
        }
    }
}
