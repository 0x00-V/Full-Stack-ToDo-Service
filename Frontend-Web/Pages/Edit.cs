using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using todolist.Models;



namespace todolist.Edit
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _http;
        public EditModel(HttpClient http)
        {
            _http = http;
        }

        [BindProperty]
        public ToDoItem Task{get; set;} = new();
        public async Task<IActionResult> OnGet(int id)
        {
            var jwt = HttpContext.Request.Cookies["jwt_session"];
            if (string.IsNullOrEmpty(jwt))
                return RedirectToPage("/Account/Login");
            var httpReqMsg = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5005/todo/get/{id}");
            httpReqMsg.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
            var response = await _http.SendAsync(httpReqMsg);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return RedirectToPage("/Index"); 
            }
            var model = await response.Content.ReadFromJsonAsync<ToDoItem>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (model == null) return RedirectToPage("/Index");
            Task = model;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if(!ModelState.IsValid) return Page();
            var jwt = HttpContext.Request.Cookies["jwt_session"];
            var httpReqMsg = new HttpRequestMessage(HttpMethod.Put, $"http://localhost:5005/todo/edit/{id}")
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
            return RedirectToPage("/index");
        }
    }  
}