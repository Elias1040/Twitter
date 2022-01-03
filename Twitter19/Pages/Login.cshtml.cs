using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

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

        }
        public IActionResult OnPost()
        {
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("UserLogin", con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (reader.GetString(1) == Email && reader.GetString(2) == Password)
                {
                    HttpContext.Session.SetString("Logged in", "1");
                    HttpContext.Session.SetInt32("ID", reader.GetInt32(0));
                    return RedirectToPage("index");
                }
            }

            return Page();
        }
    }
}
