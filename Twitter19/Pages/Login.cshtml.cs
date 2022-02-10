using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Twitter19.Classes;
using Twitter19.Repo;

namespace Twitter.Pages
{
    public class LoginModel : PageModel
    {
        #region Properties
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }
        #endregion

        #region PrivateReadonly
        private readonly IRepo _repo;
        public LoginModel(IRepo repo)
        {
            _repo = repo;
        }
        #endregion

        public void OnGet()
        {
            if (HttpContext.Session.GetString("Logged in") == "1")
            {
                HttpContext.Session.Remove("Logged in");
                HttpContext.Session.Remove("ID");
                HttpContext.Session.Remove("tweetID");
            }
        }
        public IActionResult OnPost()
        {
            int ID = _repo.Login(Email, Password);
            if (ID != 0)
            {
                HttpContext.Session.SetString("Logged in", "1");
                HttpContext.Session.SetInt32("ID", ID);
                return RedirectToPage("index");
            }
            return Page();
        }
    }
}
