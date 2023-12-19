using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Security.Claims;

namespace KafkaSignalRFrontend.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddDays(1);
            cookieOptions.Path = "/";
            
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            Response.Cookies.Append("CustomUserIdProviderCookie", "1", cookieOptions);         
        }
    }
}
