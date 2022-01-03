using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Twitter.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly Session session;
        private readonly string connectionString;
        public IndexModel(ILogger<IndexModel> logger, Session session, IConfiguration config)
        {
            _logger = logger;
            this.session = session;
            connectionString = config.GetConnectionString("Default");
        }
        
        [BindProperty]
        public string tweet { get; set; }
        public List<string> tweets { get; set; }

        public void OnGet()
        {
            session.LoggedIn();
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("GetTweets", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            con.Open();
            tweets = new List<string>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tweets.Add(reader.GetString(1));
            }
            
        }

        public IActionResult OnPost()
        {
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("CreateTweet", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            con.Open();
            cmd.Parameters.AddWithValue("@tweet", tweet);
            cmd.Parameters.AddWithValue("@user", HttpContext.Session.GetInt32("ID"));
            cmd.ExecuteNonQuery();
            return RedirectToPage();
        }
    }
}
