using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NodaTime;
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
        public List<TweetPosts> tweets { get; set; }


        public IActionResult OnGet()
        {
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("GetTweets", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            con.Open();
            tweets = new List<TweetPosts>();
            List<DateTime> date = new List<DateTime>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                TweetPosts tweetPost = new TweetPosts();
                tweetPost.name = reader.GetString(0);
                tweetPost.message = reader.GetString(1);
                date.Add(reader.GetDateTime(2));
                tweets.Add(tweetPost);
            }
            for (int i = 0; i < tweets.Count; i++)
            {
                LocalDate start = new LocalDate(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                LocalDate end = new LocalDate(date[i].Year, date[i].Month, date[i].Day);
                if (DateTime.Now.Subtract(date[i]).TotalMinutes < 1)
                {
                    tweets[i].date = "now";
                }
                else if (DateTime.Now.Subtract(date[i]).TotalMinutes > 0 && DateTime.Now.Subtract(date[i]).TotalMinutes < 60)
                {
                    tweets[i].date = $"{((int)DateTime.Now.Subtract(date[i]).TotalMinutes)} Mins";
                }
                else if (DateTime.Now.Subtract(date[i]).Hours > 0 && DateTime.Now.Subtract(date[i]).Hours < 24)
                {
                    tweets[i].date = $"{((int)DateTime.Now.Subtract(date[i]).TotalHours)} Hours";
                }
                else if (Period.Between(start, end).Days > 0 && Period.Between(start, end).Days < DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month))
                {
                    tweets[i].date = $"{Period.Between(start, end).Days} Days";
                }
                else if (Period.Between(start, end).Months > 0 && Period.Between(start, end).Months < 12)
                {
                    tweets[i].date = $"{Period.Between(start, end).Months} Months";
                }
                else
                {
                    tweets[i].date = $"{Period.Between(start, end)} Years";
                }

            }
            return session.LoggedIn();

        }

        public class TweetPosts
        {
            public string message { get; set; }
            public string name { get; set; }
            public string date { get; set; }
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
            return session.LoggedIn();
        }
    }
}
