using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Twitter19.Classes;
using Twitter19.Repo;

namespace Twitter.Pages
{
    public class IndexModel : PageModel
    {
        #region privateReadonly
        private readonly IRepo _repo;
        public IndexModel(IRepo repo)
        {
            _repo = repo;
        }
        #endregion

        #region properties
        [BindProperty]
        public string Tweet { get; set; }
        [BindProperty]
        public IFormFile Img { get; set; }
        [BindProperty]
        public string Comment { get; set; }
        public List<ListPost> Posts { get; set; }
        public List<ListComment> Comments { get; set; }
        public List<ListPost> SPosts { get; set; }
        public List<bool> Sentiment { get; set; }
        public List<bool> CommentSentiment { get; set; }
        public List<int> SentimentCount { get; set; }
        public List<int> CommentSentimentCount { get; set; }
        public List<int> CommentCount { get; set; }
        public int TweetID { get; set; }
        #endregion

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");
            
            if (id != 0)
                HttpContext.Session.SetInt32("tweetID", id);
            TweetID = id;

            #region Tweets
            int uid = (int)HttpContext.Session.GetInt32("ID");
            int tid = Convert.ToInt32(id);
            Posts = _repo.GetAllTweets();
            Sentiment = _repo.Sentiment(uid, Posts);
            SentimentCount = _repo.SentimentCount(Posts);
            CommentCount = _repo.CommentCount(Posts);
            #endregion

            #region Comments
            if (id != 0)
            {
                SPosts = _repo.GetSinglePost(tid);
                Comments = _repo.GetComments(tid);
                CommentSentiment = _repo.CommentSentiment(uid, tid);
                CommentSentimentCount = _repo.CommentSentimentCount(Comments, tid);
            }
            #endregion
            return Page();
        }



        public IActionResult OnPostTweet()
        {
            HttpContext.Session.Remove("tweetID");
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");

            if (Tweet != null)
                _repo.PostTweet((int)HttpContext.Session.GetInt32("ID"), Tweet, Img);

            return RedirectToPage("Index");
        }

        public IActionResult OnPostComment()
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");

            if (Comment != null)
                _repo.PostComment((int)HttpContext.Session.GetInt32("ID"), (int)HttpContext.Session.GetInt32("tweetID"), Comment);
            
            return RedirectToPage("Index", new { id = HttpContext.Session.GetInt32("tweetID") });
        }
    }
}
