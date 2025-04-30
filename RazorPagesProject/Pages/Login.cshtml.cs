using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using RazorPagesProject.Models;

namespace RazorPagesProject.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty] public string Username { get; set; }
        [BindProperty] public string Password { get; set; }
        public string ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            var path = Path.Combine("wwwroot", "data", "users.json");
            var jsonData = System.IO.File.ReadAllText(path);
            var users = JsonSerializer.Deserialize<List<User>>(jsonData);

            var user = users.FirstOrDefault(u => u.Username == Username && u.Password == Password && u.IsActive);

            if (user != null)
            {
                var token = Guid.NewGuid().ToString();

                // Session
                HttpContext.Session.SetString("username", user.Username);
                HttpContext.Session.SetString("token", token);
                HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

                // Cookie
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };
                Response.Cookies.Append("username", user.Username, cookieOptions);
                Response.Cookies.Append("token", token, cookieOptions);
                Response.Cookies.Append("session_id", HttpContext.Session.Id, cookieOptions);

                return RedirectToPage("Index"); // Giriş sonrası yönlendirme
            }

            ErrorMessage = "Username or password is incorrect.";
            return Page();
        }
    }
}