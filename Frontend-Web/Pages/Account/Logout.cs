using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


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