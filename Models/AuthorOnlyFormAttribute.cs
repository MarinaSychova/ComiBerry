using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace ComiBerry.Models
{
    public class AuthorOnlyFormFilter(ApplicationDbContext context) : IActionFilter
    {
        private readonly ApplicationDbContext _context = context;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            IFormCollection form = context.HttpContext.Request.Form;
            if (!form.ContainsKey("seriesId"))
            {
                context.Result = new ForbidResult();
                return;
            }

            Guid seriesId;
            try
            {
                seriesId = Guid.Parse(form["seriesId"]!);
            }
            catch
            {
                context.Result = new ForbidResult();
                return;
            }
            User user = (User)context.HttpContext!.Items["CurrentUser"]!;
            List<Series> series = [.. _context.Series.Include(s => s.User).Where(x => x.SeriesId == seriesId)];
            if (series.IsNullOrEmpty() || user.Series.IsNullOrEmpty() || series[0].User != user)
            {
                context.Result = new ForbidResult();
                return;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
    public class AuthorOnlyFormAttribute : TypeFilterAttribute
    {
        public AuthorOnlyFormAttribute() : base(typeof(AuthorOnlyFormFilter)) { }
    }
}
