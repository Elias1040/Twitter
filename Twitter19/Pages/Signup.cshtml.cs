using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Twitter19.Pages
{
    public class SignupModel : PageModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string CPassword { get; set; }
        public void OnGet()
        {

        }
        public IActionResult OnPost()
        {
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            con.Open();
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@Password", Password);
            cmd.ExecuteNonQuery();
            con.Close();
            HttpContext.Session.SetString("Logged in", "1");
            return RedirectToPage("index");
        }
    }
}
