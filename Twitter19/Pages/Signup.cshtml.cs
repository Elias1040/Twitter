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

namespace Twitter.Pages
{
    public class SignupModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string CPassword { get; set; }
        [BindProperty]
        public string Name { get; set; }
        public int id { get; set; }
        public bool exist { get; set; }
        private readonly string connectionString;
        public SignupModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("Default");
        }
        public void OnGet()
        {

        }
        public IActionResult OnPost()
        {
            //string special = @"/[!@#$%^&*()_+\-=\[\]{};':\\|,.<>\/?]/g";
            //Regex req = new Regex(@"[a-z][A-Z][0-9]");
            Regex lower = new Regex(@"[a-z]+");
            Regex upper = new Regex(@"[A-Z]+");
            Regex number = new Regex(@"[0-9]+");
            bool valid = lower.IsMatch(Password) && upper.IsMatch(Password) && number.IsMatch(Password) && Password == CPassword && Password.Length >= 8 && Password.Length < 100 && Name.Length > 0 && Name.Length < 100;

            if (valid)
            {
                SqlConnection con = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("userSignup", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@Password", Password);
                cmd.Parameters.AddWithValue("@Name", Name);
                id = cmd.ExecuteNonQuery();
                if (id != 0)
                {
                    con.Close();
                    exist = false;
                    HttpContext.Session.SetString("Logged in", "1");
                    HttpContext.Session.SetInt32("ID", id);
                    return RedirectToPage("index");
                }
                else
                {
                    exist = true;
                    con.Close();
                    return Page();
                }
            }
            else
            {
                return Page();
            }
        }
    }
}
