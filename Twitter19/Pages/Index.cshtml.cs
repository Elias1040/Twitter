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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Twitter19.Classes;
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
        public string tweet { get; set; }
        [BindProperty]
        public IFormFile img { get; set; }
        [BindProperty]
        public string comment { get; set; }
        public List<ListPost> posts { get; set; }
        public List<ListComment> comments { get; set; }
        public List<ListPost> sPosts { get; set; }
        public string tweetID { get; set; }
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

            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("GetTweets", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            con.Open();
            posts = new List<ListPost>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ListPost listPost = new ListPost();
                listPost.name = reader.GetString(0);
                listPost.message = reader.GetString(1);
                listPost.tweetID = reader.GetInt32(2);
                listPost.date = new PostDate().idk(reader.GetDateTime(3));
                try
                {
                    MemoryStream ms = new MemoryStream((byte[])reader[4]);
                    Image img = Image.FromStream(ms);
                    if (img.Width >= 500 || img.Height >= 500)
                    {
                        Image reImg = new Images().Resize(new Bitmap(img), new Size(500, 400));
                        listPost.image = new Images().ConvertToB64(reImg);
                    }
                    else
                        listPost.image = Convert.ToBase64String((byte[])reader[4]);

                }
                catch (Exception)
                { }
                posts.Add(listPost);
            }
            reader.Close();
            #region date
            
            #endregion
            posts.Reverse();
            #endregion

            #region OnGetSinglePost
            if (id != null)
            {
                tweetID = id;
                cmd = new SqlCommand("GetSingleTweet", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tweetID", id);
                reader = cmd.ExecuteReader();
                sPosts = new List<ListPost>();
                while (reader.Read())
                {
                    ListPost listPost = new ListPost();
                    listPost.name = reader.GetString(0);
                    listPost.message = reader.GetString(1);
                    try
                    {
                        MemoryStream ms = new MemoryStream((byte[])reader[2]);
                        Image img = Image.FromStream(ms);
                        if (img.Width >= 500 || img.Height >= 500)
                        {
                            Image reImg = new Images().Resize(new Bitmap(img), new Size(300, 150));
                            listPost.image = new Images().ConvertToB64(reImg);
                        }
                        else
                            listPost.image = Convert.ToBase64String((byte[])reader[4]);
                    }
                    catch (Exception)
                    { }
                    sPosts.Add(listPost);
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
                cmd = new SqlCommand("GetComments", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", HttpContext.Session.GetString("tweetID"));
                comments = new List<ListComment>();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ListComment listComment = new();
                    listComment.name = reader.GetString(0);
                    listComment.comment = reader.GetString(1);
                    listComment.date = new PostDate().idk(reader.GetDateTime(2));
                    comments.Add(listComment);
                }
                #region date
                
                #endregion
                comments.Reverse();
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

            if (tweet != null)
            {
                SqlConnection con = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("CreateTweet", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@tweet", tweet);
                cmd.Parameters.AddWithValue("@user", HttpContext.Session.GetInt32("ID"));
                byte[] data = new Images().ConvertToBytes(img);
                cmd.Parameters.AddWithValue("@imagebytes", data);
                MemoryStream ms = new MemoryStream();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return RedirectToPage("Index");
        }

        public IActionResult OnPostComment()
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");

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
            return RedirectToPage("Index", new { id = HttpContext.Session.GetString("tweetID") });

        }
    }
}
