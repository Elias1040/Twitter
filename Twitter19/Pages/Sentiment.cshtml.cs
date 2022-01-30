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
    public class SentimentModel : PageModel
    {
        #region privateReadonly
        private readonly string connectionString;
        public SentimentModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("Default");
        }
        #endregion

        public IActionResult OnGet(int id)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("UserSentiment", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            con.Open();
            cmd.Parameters.AddWithValue("UID", HttpContext.Session.GetInt32("ID"));
            cmd.Parameters.AddWithValue("TID", id);
            cmd.ExecuteNonQuery();
            HttpContext.Session.Remove("tweetID");
            return RedirectToPage("Index");
        }
    }
}
