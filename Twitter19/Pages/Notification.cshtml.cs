using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Twitter19.Classes;
using Twitter19.Repo;

namespace Twitter19.Pages
{
    public class NotificationModel : PageModel
    {
        #region privateReadonly
        private readonly IRepo _repo;
        public NotificationModel(IRepo repo)
        {
            _repo = repo;
        }
        #endregion

        public string Name { get; set; }
        public List<ListMessages> Messages { get; set; }

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");
            HttpContext.Session.SetString("MPage", "1");
            List<Profiles> profiles = _repo.GetFollowers((int)HttpContext.Session.GetInt32("ID"));
            Messages = _repo.GetNotifications(profiles, (int)HttpContext.Session.GetInt32("ID"));
            Messages.Sort((x, y) => DateTime.Compare(x.Date, y.Date));
            return Page();
        }
    }
}
