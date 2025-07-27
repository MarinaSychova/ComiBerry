using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace ComiBerry.Models
{
    public class AuthorOnlyFilter(ApplicationDbContext context) : IAuthorizationFilter
    {
        private readonly ApplicationDbContext _context = context;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            User user = (User)context.HttpContext!.Items["CurrentUser"]!;
            Guid seriesId;
            try
            {
                seriesId = Guid.Parse(context.HttpContext.Request.Query["seriesId"].ToString());
            }
            catch
            {
                context.Result = new ForbidResult();
                return;
            }
            List<Series> series = [.. _context.Series.Include(s => s.User).Where(x => x.SeriesId == seriesId)];
            if (series.IsNullOrEmpty() || user.Series.IsNullOrEmpty() || series[0].User != user)
            {
                context.Result = new ForbidResult();
                return;
            }
            if (context.HttpContext.Request.Query.ContainsKey("chapterId"))
            {
                Guid chapterId;
                try
                {
                    chapterId = Guid.Parse(context.HttpContext.Request.Query["chapterId"].ToString());
                }
                catch
                {
                    context.Result = new ForbidResult();
                    return;
                }
                List<Chapter> chapter = [.. _context.Chapters.Include(c => c.Series).Where(x => x.ChapterId == chapterId)];
                if (chapter.IsNullOrEmpty() || chapter[0].Series == null || chapter[0].Series != series[0])
                {
                    context.Result = new ForbidResult();
                }
            }
        }
    }

    public class AuthorOnlyAttribute : TypeFilterAttribute
    {
        public AuthorOnlyAttribute() : base(typeof(AuthorOnlyFilter)) { }
    }
}
