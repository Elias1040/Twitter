using System;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Twitter19.Classes;
using Twitter19.Repo;

namespace Twitter19.Pages
{
    public class ProfileModel : PageModel
    {
        #region privateReadonly
        private readonly IRepo _repo;
        public ProfileModel(IRepo repo)
        {
            _repo = repo;
        }
        #endregion

        [BindProperty]
        public IFormFile Header { get; set; }
        [BindProperty]
        public IFormFile Profile { get; set; }
        [BindProperty]
        public string Bio { get; set; }
        public string Name { get; set; }
        public string FBio { get; set; }
        public string B64Header { get; set; }
        public string B64Profile { get; set; }
        public int TweetsCount { get; set; }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");
            ListProfile listProfiles = _repo.GetProfile((int)HttpContext.Session.GetInt32("ID"));
            TweetsCount = _repo.CountTweets((int)HttpContext.Session.GetInt32("ID"));
            FBio = listProfiles.Bio;
            Name = listProfiles.Name;
            try
            {
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
        public IActionResult OnPost()
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");
            _repo.EditProfile((int)HttpContext.Session.GetInt32("ID"), Header, Profile, Bio);

            return RedirectToPage("Profile");
        }
    }
}
