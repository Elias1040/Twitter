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
        public int ID { get; set; }
        public bool Exist { get; set; }
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
            Regex lower = new(@"[a-z]+");
            Regex upper = new(@"[A-Z]+");
            Regex number = new(@"[0-9]+");
            bool valid = lower.IsMatch(Password) && upper.IsMatch(Password) && number.IsMatch(Password) && Password == CPassword && Password.Length >= 8 && Password.Length < 100 && Name.Length > 0 && Name.Length < 100;

            if (valid)
            {
                SqlConnection con = new(connectionString);
                SqlCommand cmd = new("UserSignup", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                string salt = Hash_Salt.CreateSalt(16);
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@Password", Hash_Salt.GenerateHash(Password, salt));
                cmd.Parameters.AddWithValue("@Name", Name);
                cmd.Parameters.AddWithValue("@Salt", salt);
                ID = (int)cmd.ExecuteScalar();
                if (ID != -1)
                {
                    Exist = false;
                    HttpContext.Session.SetString("Logged in", "1");
                    HttpContext.Session.SetInt32("ID", ID);
                    cmd = new("GetTweets", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        SqlCommand cmd1 = new("DefaultSentiment", con);
                        cmd1.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@UID", ID);
                        cmd1.Parameters.AddWithValue("@TID", reader.GetInt32(2));
                        cmd1.ExecuteNonQuery();
                    }
                    con.Close();
                    return RedirectToPage("index");
                }
                else
                {
                    Exist = true;
                    con.Close();
                    return Page();
                }
            }
            return Page();
        }
    }
}
