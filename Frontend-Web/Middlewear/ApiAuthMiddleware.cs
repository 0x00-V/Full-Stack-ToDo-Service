using DTO;

public class ApiAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HttpClient _http;

    public ApiAuthMiddleware(RequestDelegate next, HttpClient http)
    {
        _next = next;
        _http = http;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            var path = ctx.Request.Path.Value?.ToLower();
            if (path != null && !path.StartsWith("/account/login") && !path.StartsWith("/account/register") && !path.StartsWith("/serviceunavailable") && !path.StartsWith("/css") && !path.StartsWith("/js") && !path.StartsWith("/lib"))
            {
                var jwt = ctx.Request.Cookies["jwt_session"];

                if(string.IsNullOrEmpty(jwt))
                {
                    ctx.Response.Redirect("/Account/Login");
                    return;
                }
                var req = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5005/account/me");
                req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
                var resp = await _http.SendAsync(req);
                if(!resp.IsSuccessStatusCode)
                {
                    ctx.Response.Redirect("/Account/Login");
                    return;
                }
                var body = await resp.Content.ReadFromJsonAsync<UserDetailsDTO>();
                ctx.Items["Username"] = body!.Username;
            }
            await _next(ctx);
            return;
        } catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            ctx.Response.Redirect("/ServiceUnavailable");
        }
    }
}