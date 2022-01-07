using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
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
        [BindProperty]
        public IFormFile img { get; set; }
        public List<TweetPosts> tweets { get; set; }

        public class TweetPosts
        {
            public string message { get; set; }
            public string name { get; set; }
            public string image { get; set; }
            public string dimensions { get; set; }
            public string date { get; set; }
        }

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
                try
                {
                    byte[] a = (byte[])reader[3];
                    MemoryStream ms = new MemoryStream(a);
                    tweetPost.image = Convert.ToBase64String(a);
                    tweetPost.dimensions = reader.GetString(4);
                }
                catch (Exception)
                {

                }
                date.Add(reader.GetDateTime(2));
                tweets.Add(tweetPost);
            }
            //for (int i = 0; i < tweets.Count; i++)
            //{
            //    try
            //    {
            //        Console.WriteLine(tweets[i].dimensions.Split('w')[1].Split(' ')[0]);

            //    }
            //    catch (Exception)
            //    {


            //    }
            //}

            for (int i = 0; i < tweets.Count; i++)
            {
                //LocalDate start = new LocalDate(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                //LocalDate end = new LocalDate(date[i].Year, date[i].Month, date[i].Day);
                if (DateTime.Now.Subtract(date[i]).TotalMinutes < 1)
                {
                    tweets[i].date = "Now";
                }
                else if (DateTime.Now.Subtract(date[i]).TotalMinutes > 0 && DateTime.Now.Subtract(date[i]).TotalMinutes < 60)
                {
                    tweets[i].date = $"{(int)DateTime.Now.Subtract(date[i]).TotalMinutes} Mins";
                }
                else if (DateTime.Now.Subtract(date[i]).Hours > 0 && DateTime.Now.Subtract(date[i]).Hours < 24)
                {
                    tweets[i].date = $"{(int)DateTime.Now.Subtract(date[i]).Hours} Hours";
                }
                else
                {
                    tweets[i].date = $"{(int)DateTime.Now.Subtract(date[i]).TotalDays} Days";
                }


            }
            return session.LoggedIn();

        }



        public IActionResult OnPost()
        {
            if (!session.IsLoggedIn())
            {
                return session.LoggedIn();
            }
            else
            {
                SqlConnection con = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("CreateTweet", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@tweet", tweet);
                cmd.Parameters.AddWithValue("@user", HttpContext.Session.GetInt32("ID"));
                try
                {
                    MemoryStream ms = new MemoryStream();
                    img.CopyTo(ms);
                    byte[] data = ms.ToArray();
                    Image image = Image.FromStream(ms);
                    cmd.Parameters.AddWithValue("@imagebytes", data);
                    cmd.Parameters.AddWithValue("@dimensions", $"w{image.Width} h{image.Height}");
                }
                catch (Exception)
                {

                }
                cmd.ExecuteNonQuery();
                return RedirectToPage("Index");
            }
        }
    }
}
