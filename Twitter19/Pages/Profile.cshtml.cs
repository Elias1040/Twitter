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
        public string fBio { get; set; }
        public string Name { get; set; }
        public string b64Header { get; set; }
        public string b64Profile { get; set; }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");

            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("GetUser", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            con.Open();
            cmd.Parameters.AddWithValue("@id", HttpContext.Session.GetInt32("ID"));
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Name = reader.GetString(3);
                try
                {
                    Image profile = new Images().ConvertToImage((byte[])reader[4]);
                    Image header = new Images().ConvertToImage((byte[])reader[5]);
                    profile = new Images().Resize(profile, new Size(121, 121));
                    header = new Images().Resize(header, new Size(698, 200));
                    b64Profile = new Images().ConvertToB64(profile);
                    b64Header = new Images().ConvertToB64(header);
                    fBio = reader.GetString(6);
                }
                catch (Exception)
                {

                }
            }
            con.Close();
            return Page();
        }
        public IActionResult OnPost()
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");

            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("EditProfile", con);
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
