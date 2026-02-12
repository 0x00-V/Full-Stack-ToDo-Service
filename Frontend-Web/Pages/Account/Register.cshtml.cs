using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DTO;
using todolist.Models;

namespace todolist.Account
{
    
    public class RegisterModel : PageModel
    {
        private readonly HttpClient _http;
        public RegisterModel(HttpClient http)
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

            var httpReqMsg = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5005/account/register"){
                Content = JsonContent.Create(new {name = Credential.Username, password = Credential.Password})};
            var resp = await _http.SendAsync(httpReqMsg);
            Console.WriteLine(resp);
            if(!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Username taken.");
                return Page();
            }
            var loginResponse = await resp.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResponse == null)
            {
                ModelState.AddModelError("", "Invalid API response");
                return Page();
            }
            return RedirectToPage("/account/login");
        }
    }
}