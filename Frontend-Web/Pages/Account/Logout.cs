using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Net.Http.Json;

using dto;


namespace todolist.Account
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            Response.Cookies.Delete(
                "jwt_session",
                new CookieOptions { Path = "/" }
            );

            return RedirectToPage("/Account/Login");
        }
    }
}



         /*

        Junk references

        
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
        .*/