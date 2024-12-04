using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class SessionMiddleware
{
    private readonly RequestDelegate _next;

    public SessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var username = context.Session.GetString("Username");
        var avatar = context.Session.GetString("Avatar");

        if (username != null)
        {
            context.Items["Username"] = username;
            context.Items["Avatar"] = avatar ?? "/path/to/default/avatar.jpg";
        }

        await _next(context);
    }
}
