using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twitter19.Classes;

namespace Twitter19.Repo
{
    public interface IRepo
    {
        #region Tweet
        List<ListPost> GetAllTweets();
        List<int> CommentCount(List<ListPost> posts);
        List<int> SentimentCount(List<ListPost> posts);
        List<bool> Sentiment(int uid, List<ListPost> posts);
        void SetSentiment(int uid, int tid);
        void PostTweet(int uid, string tweet, IFormFile img);
        #endregion

        #region Comment
        List<ListPost> GetSinglePost(int id);
        List<ListComment> GetComments(int tid);
        List<bool> CommentSentiment(int uid, int tid);
        void SetCommentSentiment(int uid, int cid);
        List<int> CommentSentimentCount(List<ListComment> comments, int tid);
        void PostComment(int uid, int tid, string comment);
        #endregion

        #region Login
        int Login(string email, string password);
        #endregion

        #region Signup
        int Signup(string email, string password, string name);
        void DefaultSentiment(int id);
        void DefaultCommentSentiment(int id);
        #endregion

        #region Profile
        ListProfile GetProfile(int id);
        int CountTweets(int id);
        void EditProfile(int id, IFormFile hImg, IFormFile pImg, string bio);
        public void Follow(int uid, int pid);
        #endregion
    }
}
