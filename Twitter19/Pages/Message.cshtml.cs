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
    public class MessageModel : PageModel
    {
        #region privateReadonly
        private readonly IRepo _repo;
        public MessageModel(IRepo repo)
        {
            _repo = repo;
        }
        #endregion

        #region Properties
        [BindProperty]
        public string Name { get; set; }
        public List<Profiles> Profiles { get; set; }
        public int MessageID { get; set; }
        #endregion

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");
            HttpContext.Session.SetString("MPage", "1");
            Profiles = _repo.GetFollowers((int)HttpContext.Session.GetInt32("ID"));
            MessageID = id;
            return Page();
        }
    }
}
