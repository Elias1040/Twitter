using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Twitter19.Classes;
using Twitter19.Repo;

namespace Twitter19.Pages
{
    public class OtherProfilesModel : PageModel
    {
        #region privateReadonly
        private readonly IRepo _repo;
        public OtherProfilesModel(IRepo repo)
        {
            _repo = repo;
        }
        #endregion

        #region Properties
        public string Bio { get; set; }
        public string Name { get; set; }
        public string FBio { get; set; }
        public string B64Header { get; set; }
        public string B64Profile { get; set; }
        public int TweetsCount { get; set; }
        public int PID { get; set; }
        #endregion

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");
            else if (HttpContext.Session.GetInt32("ID") == id)
                return RedirectToPage("Profile");

            PID = id;
            ListProfile listProfiles = _repo.GetProfile(id);
            TweetsCount = _repo.CountTweets(id);
            Name = listProfiles.Name;
            try
            {
                FBio = listProfiles.Bio;
                Images images = new();
                Image profile = images.Resize(listProfiles.PImg, new Size(121, 121));
                Image header = images.Resize(listProfiles.HImg, new Size(698, 200));
                B64Profile = images.ConvertToB64(profile);
                B64Header = images.ConvertToB64(header);
            }
            catch (Exception)
            {

            }
            return Page();
        }
    }
}
