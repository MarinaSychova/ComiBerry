using Microsoft.AspNetCore.Authentication;

namespace ComiBerry.Services
{
    public class RequireCustomCookieMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            if (context.User.Identity!.IsAuthenticated && path is not null && !path!.StartsWith("/Navigation")
                && (!context.Request.Cookies.ContainsKey("UserId") || !context.Request.Cookies.ContainsKey("UserName") || !context.Request.Cookies.ContainsKey("UserEmail")))
            {
                await context.SignOutAsync(IdentityConstants.ApplicationScheme);
                context.Response.Redirect("/Navigation/Home");
                return;
            }

            await _next(context);
        }
    }
}
