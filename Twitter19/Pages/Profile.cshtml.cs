using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twitter;
using Twitter19.Classes;

namespace Twitter19.Pages
{
    public class ProfileModel : PageModel
    {
        #region privateReadonly
        private readonly string connectionString;
        public ProfileModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("Default");
        }
        #endregion

        [BindProperty]
        public IFormFile Header { get; set; }
        [BindProperty]
        public IFormFile Profile { get; set; }
        [BindProperty]
        public string Bio { get; set; }
        public string FBio { get; set; }
        public string Name { get; set; }
        public string B64Header { get; set; }
        public string B64Profile { get; set; }
        public int TweetsCount { get; set; }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");

            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("GetUser", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            con.Open();
            cmd.Parameters.AddWithValue("@id", HttpContext.Session.GetInt32("ID"));
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Name = reader.GetString(4);
                try
                {
                    Image profile = new Images().ConvertToImage((byte[])reader[5]);
                    Image header = new Images().ConvertToImage((byte[])reader[6]);
                    profile = new Images().Resize(profile, new Size(121, 121));
                    header = new Images().Resize(header, new Size(698, 200));
                    B64Profile = new Images().ConvertToB64(profile);
                    B64Header = new Images().ConvertToB64(header);
                    FBio = reader.GetString(7);
                }
                catch (Exception)
                {

                }
            }
            cmd = new("CountTweets", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UID", HttpContext.Session.GetInt32("ID"));
            TweetsCount = (int)cmd.ExecuteScalar();
            con.Close();
            return Page();
        }
        public IActionResult OnPost()
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");

            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("EditProfile", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            con.Open();
            byte[] headerBytes = new Images().ConvertToBytes(Header);
            byte[] profileBytes = new Images().ConvertToBytes(Profile);
            cmd.Parameters.AddWithValue("@id", HttpContext.Session.GetInt32("ID"));
            cmd.Parameters.AddWithValue("@Profile", profileBytes);
            cmd.Parameters.AddWithValue("@Header", headerBytes);
            cmd.Parameters.AddWithValue("@Bio", Bio);
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToPage("Profile");
        }
    }
}
