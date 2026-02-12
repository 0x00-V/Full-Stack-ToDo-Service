using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using todolist.Models;


namespace todolist.Create
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _http;
        public CreateModel(HttpClient http)
        {
            _http = http;
        }

        [BindProperty]
        public ToDoTask Task{get; set;} = new();
        public IActionResult OnGet()
        {
            return Page();
        }

        public async  Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();
            var jwt = HttpContext.Request.Cookies["jwt_session"];
            var httpReqMsg = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5005/todo/create")
            {Content = JsonContent.Create(new {title = Task.Title, description = Task.Description})};
            httpReqMsg.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
            var response = await _http.SendAsync(httpReqMsg);
            var statusCode = response.StatusCode;
            var responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", $"API Error: {statusCode} | {responseBody}");
                return Page();
            }
            Console.WriteLine(response);
            return RedirectToPage("/index");
        }
    }  
}