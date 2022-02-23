using System;
using System.Drawing;

namespace Twitter19.Classes
{
    public class ListPost
    {
        public string Message { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public int TweetID { get; set; }
    }

    public class ListComment
    {
        public string Comment { get; set; }
        public int CommentID { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
    }

    public class ListProfile
    {
        public Image PImg { get; set; }
        public Image HImg { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
    }

    public class Profiles
    {
        public string Name { get; set; }
        public string Img { get; set; }
        public string date { get; set; }
        public int id { get; set; }
    }

    public class ListMessages
    {
        public string Message { get; set; }
        public int UserID { get; set; }
        public int FollowerID { get; set; }
        public DateTime Date { get; set; }
    }

    public class PostDate
    {
        public string Idk(DateTime dateTime)
        {
            TimeSpan now = DateTime.Now.Subtract(dateTime);
            if (now <= TimeSpan.FromSeconds(60))
            {
                return String.Format($"Now");
            }
            else if (now <= TimeSpan.FromMinutes(60))
            {
                return String.Format($"{now.Minutes} minute/s");
            }
            else if (now <= TimeSpan.FromHours(24))
            {
                return String.Format($"{now.Hours} hour/s");
            }
            else if (now <= TimeSpan.FromDays(30))
            {
                return now.Days > 1 ?
                    String.Format($"{now.Days} day/s") :
                    "Yesterday";
            }
            else if (now <= TimeSpan.FromDays(365))
            {
                return
                    String.Format($"{now.Days / 30} month/s");
            }
            else
            {
                return String.Format($"{now.Days / 365} year/s");
            }
        }
    }
}