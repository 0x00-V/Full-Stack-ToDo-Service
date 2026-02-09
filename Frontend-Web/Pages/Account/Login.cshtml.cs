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
    
    public class LoginModel : PageModel
    {
        private readonly HttpClient _http;
        public LoginModel(HttpClient http)
        {
            _http = http;
        }

        [BindProperty]
        public Credential Credential{get; set;} = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();

            var httpReqMsg = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5005/account/login"){
                Content = JsonContent.Create(new {name = Credential.Username, password = Credential.Password})};
            var resp = await _http.SendAsync(httpReqMsg);
            if(!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return Page();
            }
            var loginResponse = await resp.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResponse == null)
            {
                ModelState.AddModelError("", "Invalid API response");
                return Page();
            }
            var jwt = loginResponse.AccessToken;
            Response.Cookies.Append("jwt_session", jwt, new CookieOptions{ HttpOnly = true, Secure = false, SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax, Expires = DateTimeOffset.UtcNow.AddSeconds(loginResponse.ExpiresIn)});
            return RedirectToPage("/index");
        }



    }
    public class Credential
    {
        [Required]
        public string Username {get; set;} = default!;
        [Required]
        [DataType(DataType.Password)]
        public string Password {get; set;} = default!;
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