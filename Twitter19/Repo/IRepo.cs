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
        List<ListPost> GetSinglePost(int id);
        List<bool> Sentiment(int uid, List<ListPost> posts);
        List<int> SentimentCount(List<ListPost> posts);
        List<int> CommentCount(List<ListPost> posts);
        void PostTweet(int uid, string tweet, IFormFile img);
        #endregion

        #region Comment
        List<ListComment> GetComments(int tid);
        List<bool> CommentSentiment(int uid, int tid);
        List<int> CommentSentimentCount(List<ListComment> comments, int tid);
        void PostComment(int uid, int tid, string comment);
        #endregion

        #region Login

        #endregion

        #region Signup

        #endregion

        #region Profile

        #endregion
    }
}
