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

namespace Twitter19.Pages
{
    public class ProfileModel : PageModel
    {
        [BindProperty]
        public IFormFile Header { get; set; }
        [BindProperty]
        public IFormFile Profile { get; set; }
        [BindProperty]
        public string Bio { get; set; }
        private readonly ILogger<ProfileModel> _logger;
        private readonly Session session;
        private readonly string connectionString;
        public ProfileModel(ILogger<ProfileModel> logger, Session session, IConfiguration config)
        {
            _logger = logger;
            this.session = session;
            connectionString = config.GetConnectionString("Default");
        }
        public IActionResult OnGet()
        {
            if (!session.IsLoggedIn())
            {
                return session.LoggedIn();
            }
            return Page();
        }
        public IActionResult OnPost()
        {
            if (!session.IsLoggedIn())
            {
                return session.LoggedIn();
            }
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("EditProfile", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            byte[] headerBytes = Converter(Header);
            byte[] profileBytes = Converter(Profile);
            cmd.Parameters.AddWithValue("@Header", headerBytes);
            cmd.Parameters.AddWithValue("@Profile", profileBytes);
            cmd.Parameters.AddWithValue("@Bio", Bio);
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToPage("Profile");
        }
        public byte[] Converter(IFormFile img)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                img.CopyTo(ms);
                ms.Close();
                return ms.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
