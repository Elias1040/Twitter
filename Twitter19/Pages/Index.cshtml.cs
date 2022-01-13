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
        [BindProperty]
        public string comment { get; set; }
        public List<TweetPosts> tweets { get; set; }
        public List<TweetComments> comments { get; set; }
        public string cMessage { get; set; }
        public string cName { get; set; }
        public string cImage { get; set; }
        public string cDimensions { get; set; }
        public string cDate { get; set; }
        public string tweetID { get; set; }

        public class TweetPosts
        {
            public string message { get; set; }
            public string name { get; set; }
            public string image { get; set; }
            public string dimensions { get; set; }
            public string date { get; set; }
            public int tweetID { get; set; }
        }

        public class TweetComments
        {
            public string comment { get; set; }
            public string name { get; set; }
            public string date { get; set; }

        }

        public IActionResult OnGet(string id)
        {
            #region OnGet1
            if (id != null)
            {
                HttpContext.Session.SetString("tweetID", id);
            }
            tweetID = id;
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
                tweetPost.tweetID = reader.GetInt32(2);
                try
                {
                    byte[] a = (byte[])reader[4];
                    MemoryStream ms = new MemoryStream(a);
                    tweetPost.image = Convert.ToBase64String(a);
                    tweetPost.dimensions = reader.GetString(5);
                }
                catch (Exception)
                {

                }
                date.Add(reader.GetDateTime(3));
                tweets.Add(tweetPost);
            }
            reader.Close();

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
            tweets.Reverse();
            #endregion
            #region OnGet2
            if (id != null)
            {
                cmd = new SqlCommand("GetSingleTweet", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tweetID", id);
                reader = cmd.ExecuteReader();
                DateTime ddate = new DateTime();
                while (reader.Read())
                {
                    cName = reader.GetString(0);
                    cMessage = reader.GetString(1);
                    try
                    {
                        byte[] a = (byte[])reader[2];
                        MemoryStream ms = new MemoryStream(a);
                        cImage = Convert.ToBase64String(a);
                        cDimensions = reader.GetString(3);
                    }
                    catch (Exception)
                    {

                    }
                    ddate = reader.GetDateTime(4);
                }
                reader.Close();

                if (DateTime.Now.Subtract(ddate).TotalMinutes < 1)
                {
                    cDate = "Now";
                }
                else if (DateTime.Now.Subtract(ddate).TotalMinutes > 0 && DateTime.Now.Subtract(ddate).TotalMinutes < 60)
                {
                    cDate = $"{(int)DateTime.Now.Subtract(ddate).TotalMinutes} Mins";
                }
                else if (DateTime.Now.Subtract(ddate).Hours > 0 && DateTime.Now.Subtract(ddate).Hours < 24)
                {
                    cDate = $"{(int)DateTime.Now.Subtract(ddate).Hours} Hours";
                }
                else
                {
                    cDate = $"{(int)DateTime.Now.Subtract(ddate).TotalDays} Days";
                }
            }
            #endregion
            #region OnGet3
            if (HttpContext.Session.GetString("tweetID") != null)
            {
                cmd = new SqlCommand("GetComments", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                comments = new List<TweetComments>();
                List<DateTime> CDate = new List<DateTime>();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    TweetComments tweetComments = new TweetComments();
                    tweetComments.name = reader.GetString(0);
                    tweetComments.comment = reader.GetString(1);
                    CDate.Add(reader.GetDateTime(2));
                    comments.Add(tweetComments);
                }

                for (int i = 0; i < comments.Count; i++)
                {
                    //LocalDate start = new LocalDate(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    //LocalDate end = new LocalDate(date[i].Year, date[i].Month, date[i].Day);
                    if (DateTime.Now.Subtract(CDate[i]).TotalMinutes < 1)
                    {
                        comments[i].date = "Now";
                    }
                    else if (DateTime.Now.Subtract(CDate[i]).TotalMinutes > 0 && DateTime.Now.Subtract(CDate[i]).TotalMinutes < 60)
                    {
                        comments[i].date = $"{(int)DateTime.Now.Subtract(CDate[i]).TotalMinutes} Mins";
                    }
                    else if (DateTime.Now.Subtract(CDate[i]).Hours > 0 && DateTime.Now.Subtract(CDate[i]).Hours < 24)
                    {
                        comments[i].date = $"{(int)DateTime.Now.Subtract(CDate[i]).Hours} Hours";
                    }
                    else
                    {
                        comments[i].date = $"{(int)DateTime.Now.Subtract(CDate[i]).TotalDays} Days";
                    }
                }
                comments.Reverse();
            }
            #endregion
            con.Close();
            return session.LoggedIn();
        }



        public IActionResult OnPostTweet()
        {
            HttpContext.Session.Remove("tweetID");
            if (!session.IsLoggedIn())
            {
                return session.LoggedIn();
            }
            else
            {
                if (tweet != null)
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
#pragma warning disable CA1416 // Validate platform compatibility
                        Image image = Image.FromStream(ms);
                        cmd.Parameters.AddWithValue("@imagebytes", data);
                        cmd.Parameters.AddWithValue("@dimensions", $"w{image.Width} h{image.Height}");
#pragma warning restore CA1416 // Validate platform compatibility
                    }
                    catch (Exception)
                    {

                    }
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                return RedirectToPage("Index");
            }
        }

        public IActionResult OnPostComment()
        {
            if (!session.IsLoggedIn())
            {
                return session.LoggedIn();
            }
            else
            {
                if (comment != null)
                {
                    SqlConnection con = new SqlConnection(connectionString);
                    SqlCommand cmd = new SqlCommand("CreateComment", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    con.Open();
                    cmd.Parameters.AddWithValue("@tweetID", HttpContext.Session.GetString("tweetID"));
                    cmd.Parameters.AddWithValue("@userID", HttpContext.Session.GetInt32("ID"));
                    cmd.Parameters.AddWithValue("@comment", comment);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                return RedirectToPage("Index");
            }
        }
    }
}
