using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Twitter19.Classes;
using Twitter19.Repo;

namespace Twitter19.Pages
{
    public class MessageModel : PageModel
    {
        #region privateReadonly

        private readonly IRepo _repo;

        public MessageModel(IRepo repo)
        {
            _repo = repo;
        }

        #endregion privateReadonly

        #region Properties

        [BindProperty]
        public string Name { get; set; }

        public List<Profiles> Profiles { get; set; }
        public List<ListMessages> Messages { get; set; }

        public int FollowerRoomID { get; set; }
        public int? RoomID { get; set; }

        #endregion Properties

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");
            HttpContext.Session.SetString("MPage", "1");
            Profiles = _repo.GetFollowers((int)HttpContext.Session.GetInt32("ID"));
            FollowerRoomID = id;
            if (id != 0)
            {
                Messages = _repo.GetMessages((int)HttpContext.Session.GetInt32("ID"), id);
                Messages.AddRange(_repo.GetMessages(id, (int)HttpContext.Session.GetInt32("ID")));
                Messages.Sort((x, y) => DateTime.Compare(x.Date, y.Date));
            }
            return Page();
        }
    }
}