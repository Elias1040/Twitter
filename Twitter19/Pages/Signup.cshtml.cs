using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Twitter19.Classes;
using Twitter19.Repo;

namespace Twitter.Pages
{
    public class SignupModel : PageModel
    {
        #region Properties
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string CPassword { get; set; }
        [BindProperty]
        public string Name { get; set; }
        public bool Exist { get; set; }
        #endregion

        #region PrivateReadonly
        private readonly IRepo _repo;
        public SignupModel(IRepo repo)
        {
            _repo = repo;
        }
        #endregion

        public void OnGet()
        {

        }
        public IActionResult OnPost()
        {
            //string special = @"/[!@#$%^&*()_+\-=\[\]{};':\\|,.<>\/?]/g";
            //Regex req = new Regex(@"[a-z][A-Z][0-9]");
            Regex lower = new(@"[a-z]+");
            Regex upper = new(@"[A-Z]+");
            Regex number = new(@"[0-9]+");
            bool valid = lower.IsMatch(Password) && upper.IsMatch(Password) && number.IsMatch(Password) && Password == CPassword && Password.Length >= 8 && Password.Length < 100 && Name.Length > 0 && Name.Length < 100;

            if (valid)
            {
                int ID = _repo.Signup(Email, Password, Name);
                if (ID != 0)
                {
                    Exist = false;
                    HttpContext.Session.SetString("Logged in", "1");
                    HttpContext.Session.SetInt32("ID", ID);
                    _repo.DefaultSentiment(ID);
                    _repo.DefaultCommentSentiment(ID);
                    return RedirectToPage("index");
                }
                else
                {
                    Exist = true;
                }
            }
            return Page();
        }
    }
}
