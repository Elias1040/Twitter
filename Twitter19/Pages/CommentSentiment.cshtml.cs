using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Twitter19.Repo;

namespace Twitter19.Pages
{
    public class CommentSentimentModel : PageModel
    {
        #region privateReadonly
        private readonly IRepo _repo;
        public CommentSentimentModel(IRepo repo)
        {
            _repo = repo;
        }
        #endregion
        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");

            _repo.SetCommentSentiment((int)HttpContext.Session.GetInt32("ID"), id);
            string rTID = HttpContext.Session.GetString("tweetID");
            HttpContext.Session.Remove("tweetID");
            return RedirectToPage("Index", new { id = rTID });
        }
    }
}
