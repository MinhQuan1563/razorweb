using Microsoft.AspNetCore.Authorization;

namespace ASP_EF.Security.Requirements
{
    public class ArticleUpdateRequirement : IAuthorizationRequirement
    {
        public ArticleUpdateRequirement(int year = 2023, int month = 7, int day = 30)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }
}
