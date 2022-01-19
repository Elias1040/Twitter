using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Twitter19.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Remove("Logged in");
            HttpContext.Session.Remove("ID");
            HttpContext.Session.Remove("tweetID");
            return RedirectToPage("Login");
        }
    }
}
