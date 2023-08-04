using ASP_EF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_EF.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly MyBlogContext myBlogContext;

        public IndexModel(ILogger<IndexModel> logger, MyBlogContext _myBlogContext)
        {
            _logger = logger;
            myBlogContext = _myBlogContext;
        }

        public void OnGet()
        {
            var posts = from ar in myBlogContext.articles
                        orderby ar.Created descending
                        select ar;

            ViewData["posts"] = posts.ToList();
        }
    }
}