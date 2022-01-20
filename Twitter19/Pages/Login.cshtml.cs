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

namespace Twitter.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }
        private readonly string connectionString;
        public LoginModel(IConfiguration config) 
        { 
            connectionString = config.GetConnectionString("Default"); 
        }
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
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("UserLogin", con);
            con.Open();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", Email);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (Hash_Salt.PasswordAreEqual(Password, reader.GetString(2), reader.GetString(3)))
                {
                    HttpContext.Session.SetString("Logged in", "1");
                    HttpContext.Session.SetInt32("ID", reader.GetInt32(0));
                    return RedirectToPage("Index");
                }
            }
            return Page();
        }
    }
}
