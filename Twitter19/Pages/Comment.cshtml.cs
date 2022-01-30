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
    public class CommentModel : PageModel
    {
        #region privateReadonly
        private readonly string connectionString;
        public CommentModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("Default");
        }
        #endregion

        public IActionResult OnGet(string id, string Comment)
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");

            if (Comment != null)
            {
                SqlConnection con = new(connectionString);
                SqlCommand cmd = new("CreateComment", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@tweetID", HttpContext.Session.GetString("tweetID"));
                cmd.Parameters.AddWithValue("@userID", HttpContext.Session.GetInt32("ID"));
                cmd.Parameters.AddWithValue("@comment", Comment);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return RedirectToPage("Index", new { id = HttpContext.Session.GetString("tweetID") });
        }
    }
}
