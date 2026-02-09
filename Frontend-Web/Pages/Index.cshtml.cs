using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;

namespace Frontend_Web.Pages;

public class IndexModel : PageModel
{

    private readonly IHttpClientFactory _httpClientFactory;
    public IndexModel(IHttpClientFactory httpclientFactory) => _httpClientFactory = httpclientFactory;
    public string? Username { get; private set; }
    public async Task<IActionResult> OnGet()
    {
        Username = HttpContext.Items["Username"] as string;
        return Page();
    }
}
