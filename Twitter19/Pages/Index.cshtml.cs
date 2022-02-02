using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Twitter19.Classes;

namespace Twitter.Pages
{
    public class IndexModel : PageModel
    {
        #region privateReadonly
        private readonly string connectionString;
        public IndexModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("Default");
        }
        #endregion

        #region properties
        [BindProperty]
        public string Tweet { get; set; }
        [BindProperty]
        public IFormFile Img { get; set; }
        [BindProperty]
        public string Comment { get; set; }
        public List<ListPost> Posts { get; set; }
        public List<ListComment> Comments { get; set; }
        public List<ListPost> SPosts { get; set; }
        public List<bool> Sentiment { get; set; }
        public List<bool> CommentSentiment { get; set; }
        public List<int> SentimentCount { get; set; }
        public List<int> CommentSentimentCount { get; set; }
        public string TweetID { get; set; }
        #endregion

        public IActionResult OnGet(string id)
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
            {
                return RedirectToPage("Login");
            }

            #region OnGetPosts
            if (id != null)
                HttpContext.Session.SetString("tweetID", id);
            TweetID = id;

            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("GetTweets", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            con.Open();
            Posts = new List<ListPost>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ListPost listPost = new();
                listPost.Name = reader.GetString(0);
                listPost.Message = reader.GetString(1);
                listPost.TweetID = reader.GetInt32(2);
                listPost.Date = new PostDate().Idk(reader.GetDateTime(3));
                try
                {
                    MemoryStream ms = new((byte[])reader[4]);
                    Image img = Image.FromStream(ms);
                    if (img.Width >= 500 || img.Height >= 500)
                    {
                        Image reImg = new Images().Resize(new Bitmap(img), new Size(500, 400));
                        listPost.Image = new Images().ConvertToB64(reImg);
                    }
                    else
                        listPost.Image = Convert.ToBase64String((byte[])reader[4]);

                }
                catch (Exception)
                { }
                Posts.Add(listPost);
            }
            reader.Close();
            cmd = new("GetSentiment", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UID", HttpContext.Session.GetInt32("ID"));
            Sentiment = new List<bool>();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Sentiment.Add(reader.GetBoolean(0));
            }
            reader.Close();
            SentimentCount = new List<int>();
            foreach (var item in Posts)
            {
                cmd = new("CountSentiment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TID", item.TweetID);
                SentimentCount.Add((int)cmd.ExecuteScalar());
            }
            Posts.Reverse();
            Sentiment.Reverse();
            SentimentCount.Reverse();
            #endregion

            #region OnGetSinglePost
            if (id != null)
            {
                cmd = new("GetSingleTweet", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tweetID", id);
                reader = cmd.ExecuteReader();
                SPosts = new List<ListPost>();
                while (reader.Read())
                {
                    ListPost listPost = new();
                    listPost.Name = reader.GetString(0);
                    listPost.Message = reader.GetString(1);
                    try
                    {
                        MemoryStream ms = new((byte[])reader[2]);
                        Image img = Image.FromStream(ms);
                        if (img.Width >= 500 || img.Height >= 500)
                        {
                            Image reImg = new Images().Resize(new Bitmap(img), new Size(300, 150));
                            listPost.Image = new Images().ConvertToB64(reImg);
                        }
                        else
                            listPost.Image = Convert.ToBase64String((byte[])reader[4]);
                    }
                    catch (Exception)
                    { }
                    SPosts.Add(listPost);
                }
                reader.Close();
                #region date

                #endregion
            }
            #endregion

            #region OnGetComments
            //string test = HttpContext.Session.GetString("tweetID");
            if (id != null)
            {
                cmd = new("GetComments", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", HttpContext.Session.GetString("tweetID"));
                Comments = new List<ListComment>();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ListComment listComment = new();
                    listComment.Name = reader.GetString(0);
                    listComment.Comment = reader.GetString(1);
                    listComment.Date = new PostDate().Idk(reader.GetDateTime(3));
                    listComment.CommentID = reader.GetInt32(2);
                    Comments.Add(listComment);
                }
                reader.Close();
                cmd = new("GetCommentSentiment", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UID", HttpContext.Session.GetInt32("ID"));
                cmd.Parameters.AddWithValue("@TID", id);
                CommentSentiment = new List<bool>();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    CommentSentiment.Add(reader.GetBoolean(0));
                }
                reader.Close();

                CommentSentimentCount = new List<int>();
                foreach (var item in Comments)
                {
                    cmd = new("CountCommentSentiment", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CID", item.CommentID);
                    cmd.Parameters.AddWithValue("@TID", id);
                    CommentSentimentCount.Add((int)cmd.ExecuteScalar());
                }
                Comments.Reverse();
                CommentSentiment.Reverse();
                CommentSentimentCount.Reverse();
            }
            #endregion
            con.Close();
            return Page();
        }



        public IActionResult OnPostTweet()
        {
            HttpContext.Session.Remove("tweetID");
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");

            if (Tweet != null)
            {
                SqlConnection con = new(connectionString);
                SqlCommand cmd = new("CreateTweet", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                int id = (int)HttpContext.Session.GetInt32("ID");
                cmd.Parameters.AddWithValue("@tweet", Tweet);
                cmd.Parameters.AddWithValue("@user", HttpContext.Session.GetInt32("ID"));
                byte[] data = new Images().ConvertToBytes(Img);
                cmd.Parameters.AddWithValue("@imagebytes", data);
                int TID = (int)cmd.ExecuteScalar();
                Console.WriteLine(TID);
                con.Close();
                con.Open();
                cmd = new("GetAllUserIDs", con);
                List<int> Users = new();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Users.Add(reader.GetInt32(0));
                }
                con.Close();
                con.Open();
                foreach (var item in Users)
                {
                    cmd = new("DefaultSentiment", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UID", item);
                    cmd.Parameters.AddWithValue("@TID", TID);
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            return RedirectToPage("Index");
        }

        public IActionResult OnPostComment()
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
                int CID = (int)cmd.ExecuteScalar();
                List<int> Users = new();
                cmd = new("GetAllUserIDs", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Users.Add(reader.GetInt32(0));
                }
                foreach (var item in Users)
                {
                    string a = HttpContext.Session.GetString("tweetID");
                    cmd = new("DefaultCommentSentiment", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UID", item);
                    cmd.Parameters.AddWithValue("@CID", CID);
                    cmd.Parameters.AddWithValue("@TID", HttpContext.Session.GetString("tweetID"));
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            return RedirectToPage("Index", new { id = HttpContext.Session.GetString("tweetID") });
        }
    }
}
