using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace todolist.Account
{
    public class LoginModel : PageModel
    {
        public Credential Credential{get; set;} = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();
            return Page();
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