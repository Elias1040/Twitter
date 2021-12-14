using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Twitter19.Pages
{
    public class SignupModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string CPassword { get; set; }
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
            if (Password == CPassword)
            {
                SqlConnection con = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("UserSignup", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@Password", Password);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    con.Close();
                    HttpContext.Session.SetString("Logged in", "1");
                    return RedirectToPage("index");
                }
                else
                {
                    con.Close();
                    return Page();
                }
                //cmd.ExecuteNonQuery();
            }
            else
            {
                return Page();
            }
        }
    }
}
