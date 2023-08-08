using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ASP_EF.Models;
using Microsoft.IdentityModel.Tokens;

namespace ASP_EF.Pages.Blog
{
    public class IndexModel : PageModel
    {
        private readonly ASP_EF.Models.MyBlogContext _context;

        public IndexModel(ASP_EF.Models.MyBlogContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get;set; } = default!;

        public const int ITEMS_PER_PAGE = 5;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }

        public async Task OnGetAsync(string SearchString)
        {
            var totalArticle = await _context.articles.CountAsync();
            TotalPage = (int)Math.Ceiling((double)totalArticle / ITEMS_PER_PAGE);

            if(CurrentPage < 1)
                CurrentPage = 1;
            if(CurrentPage > TotalPage)
                CurrentPage = TotalPage;

            if (_context.articles != null)
            {
                // Article = await _context.articles.ToListAsync();

                var qr = (from article in _context.articles
                         orderby article.Created descending
                         select article)
                         .Skip((CurrentPage - 1) * ITEMS_PER_PAGE)
                         .Take(ITEMS_PER_PAGE);

                if(!SearchString.IsNullOrEmpty())
                {
                    Article = await qr.Where(q => q.Title.Contains(SearchString)).ToListAsync();
                }
                else
                    Article = await qr.ToListAsync();

            }
        }
    }
}
