using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;

namespace Frontend_Web.Pages;

public class IndexModel : PageModel
{

    private readonly IHttpClientFactory _httpClientFactory;
    public IndexModel(IHttpClientFactory httpclientFactory) => _httpClientFactory = httpclientFactory;
    public async Task<IActionResult> OnGet()
    {

        var jwt_session = Request.Cookies["jwt_session"];
        if(string.IsNullOrEmpty(jwt_session))
        {
            return new RedirectToPageResult("/account/login");
        }

        
        //Response.Cookies.Append("jwt_session", "Bearer test", new CookieOptions{ HttpOnly = true, Secure = false, SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict, Expires = DateTimeOffset.UtcNow.AddMinutes(45)});

        var httpRequestMessage_CheckJWTIsValid = new HttpRequestMessage(
            HttpMethod.Post, "http://localhost:5005/account/testauth")
        {
            Headers = {
                {HeaderNames.Authorization, jwt_session}
                }
        };
        // Note to readers - Just discovering how to use client factory, ignore the nonexistent logic
        var httpClient = _httpClientFactory.CreateClient();
        var httpResponseMessage = httpClient.Send(httpRequestMessage_CheckJWTIsValid);
        var responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
        var statusCode = (int)httpResponseMessage.StatusCode;
        var isSuccess  = httpResponseMessage.IsSuccessStatusCode;
        //Console.WriteLine(httpResponseMessage);
        Console.WriteLine(statusCode);
        Console.WriteLine(isSuccess);
        Console.WriteLine(responseBody);
        return Page();
    }
}
