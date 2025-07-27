namespace ComiBerry.Services
{
    public class RequireValidUserMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var user = await userManager.GetUserAsync(context.User);

                if (user is null)
                {
                    await signInManager.SignOutAsync();
                    context.Response.Cookies.Delete("UserId");
                    context.Response.Cookies.Delete("UserName");
                    context.Response.Cookies.Delete("UserEmail");
                    context.Response.Redirect("/Navigation/Home");
                    return;
                }
                context.Items["CurrentUser"] = user;
            }

            await _next(context);
        }
    }
}
