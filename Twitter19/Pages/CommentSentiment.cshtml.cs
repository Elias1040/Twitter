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
    public class CommentSentimentModel : PageModel
    {
        #region privateReadonly
        private readonly string connectionString;
        public CommentSentimentModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("Default");
        }
        #endregion
        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");

            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("UserCommentSentiment", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            con.Open();
            cmd.Parameters.AddWithValue("UID", HttpContext.Session.GetInt32("ID"));
            cmd.Parameters.AddWithValue("CID", id);
            cmd.ExecuteNonQuery();
            string rTID = HttpContext.Session.GetString("tweetID");
            HttpContext.Session.Remove("tweetID");
            return RedirectToPage("Index", new { id = rTID });

        }
    }
}
