using System.Security.Cryptography.X509Certificates;
using dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;

namespace Frontend_Web.Pages;

public class IndexModel : PageModel
{
    public List<ToDoItem> Items { get; private set; } = new();

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _http;
    public IndexModel(IHttpClientFactory httpclientFactory, HttpClient http)
    {
        _http = http;
        _httpClientFactory = httpclientFactory;
    }
    public string? Username { get; private set; }
    public async Task<IActionResult> OnGet()
    {
        Username = HttpContext.Items["Username"] as string;

        string? jwt = HttpContext.Request.Cookies["jwt_session"];
        var req = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5005/todo/get");
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        var resp = await _http.SendAsync(req);
        if(!resp.IsSuccessStatusCode)
        {
           //Console.WriteLine("Error"); 
           return Page();
        }
        Items = await resp.Content.ReadFromJsonAsync<List<ToDoItem>>() ?? new List<ToDoItem>();
        return Page();
    }
}
