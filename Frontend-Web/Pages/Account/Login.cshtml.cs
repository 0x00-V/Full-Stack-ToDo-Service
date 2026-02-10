using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using dto;
using todolist.Models;


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
}