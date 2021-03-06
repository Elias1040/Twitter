using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using Twitter19.Classes;

namespace Twitter19.Repo
{
    public class Repo : IRepo
    {
        #region PrivateReadonly

        private readonly string connectionString;

        public Repo(IConfiguration config)
        {
            connectionString = config.GetConnectionString("Default");
        }

        #endregion PrivateReadonly

        #region Tweet

        public List<ListPost> GetAllTweets()
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("GetTweets", con);
            cmd.CommandType = CommandType.StoredProcedure;
            List<ListPost> Posts = new();
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            foreach (DataRow item in dt.Rows)
            {
                ListPost listPost = new ListPost();
                listPost.Name = (string)item[0];
                try
                {
                    MemoryStream ms = new MemoryStream((byte[])item[1]);
                    Image img = Image.FromStream(ms);

                    if (img.Width >= 75 || img.Height >= 75)
                    {
                        Image reImg = new Images().Resize(new Bitmap(img), new Size(75, 75));
                        listPost.PImg = new Images().ConvertToB64(reImg);
                    }
                }
                catch (Exception)
                {


                }
                listPost.Message = (string)item[2];
                listPost.TweetID = (int)item[3];
                listPost.Date = new PostDate().Idk((DateTime)item[4]);
                try
                {
                    MemoryStream ms = new MemoryStream((byte[])item[5]);
                    Image img = Image.FromStream(ms);
                    if (img.Width >= 500 || img.Height >= 500)
                    {
                        Image reImg = new Images().Resize(new Bitmap(img), new Size(500, 400));
                        listPost.Image = new Images().ConvertToB64(reImg);
                    }
                    else
                        listPost.Image = Convert.ToBase64String((byte[])item[5]);
                }
                catch (Exception)
                {
                }
                Posts.Add(listPost);
            }
            //SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    ListPost listPost = new();
            //    listPost.Name = reader.GetString(0);
            //    listPost.Message = reader.GetString(1);
            //    listPost.TweetID = reader.GetInt32(2);
            //    listPost.Date = new PostDate().Idk(reader.GetDateTime(3));
            //    try
            //    {
            //        MemoryStream ms = new((byte[])reader[4]);
            //        Image img = Image.FromStream(ms);
            //        if (img.Width >= 500 || img.Height >= 500)
            //        {
            //            Image reImg = new Images().Resize(new Bitmap(img), new Size(500, 400));
            //            listPost.Image = new Images().ConvertToB64(reImg);
            //        }
            //        else
            //            listPost.Image = Convert.ToBase64String((byte[])reader[4]);

            //    }
            //    catch (Exception)
            //    { }
            //    Posts.Add(listPost);
            //}
            con.Close();
            Posts.Reverse();
            return Posts;
        }

        public List<bool> Sentiment(int uid, List<ListPost> posts)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("GetSentiment", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UID", uid);
            List<bool> Sentiment = new();
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            foreach (DataRow item in dt.Rows)
            {
                Sentiment.Add((bool)item[0]);
            }
            //SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    Sentiment.Add(reader.GetBoolean(0));
            //}
            con.Close();
            Sentiment.Reverse();
            return Sentiment;
        }

        public List<int> SentimentCount(List<ListPost> posts)
        {
            SqlConnection con = new(connectionString);
            List<int> SentimentCount = new();
            foreach (var item in posts)
            {
                SqlCommand cmd = new("CountSentiment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TID", item.TweetID);
                con.Open();
                SentimentCount.Add((int)cmd.ExecuteScalar());
                con.Close();
            }
            return SentimentCount;
        }

        public List<int> CommentCount(List<ListPost> posts)
        {
            SqlConnection con = new(connectionString);
            List<int> CommentCount = new();
            foreach (var item in posts)
            {
                SqlCommand cmd = new("CountComments", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TID", item.TweetID);
                con.Open();
                CommentCount.Add((int)cmd.ExecuteScalar());
                con.Close();
            }
            return CommentCount;
        }

        public void SetSentiment(int uid, int tid)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("UserSentiment", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("UID", uid);
            cmd.Parameters.AddWithValue("TID", tid);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void PostTweet(int uid, string tweet, IFormFile img)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("CreateTweet", con);
            byte[] data = new Images().ConvertToBytes(img);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tweet", tweet);
            cmd.Parameters.AddWithValue("@user", uid);
            cmd.Parameters.AddWithValue("@imagebytes", data);
            con.Open();
            int TID = (int)cmd.ExecuteScalar();
            con.Close();
            cmd = new("GetAllUserIDs", con);
            List<int> Users = new();
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            foreach (DataRow item in dt.Rows)
            {
                Users.Add((int)item[0]);
            }
            //SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    Users.Add(reader.GetInt32(0));
            //}
            con.Close();
            foreach (var item in Users)
            {
                cmd = new("DefaultSentiment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UID", item);
                cmd.Parameters.AddWithValue("@TID", TID);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        #endregion Tweet

        #region Comment

        public List<ListPost> GetSinglePost(int id)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("GetSingleTweet", con);
            List<ListPost> Posts = new();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tweetID", id);
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            foreach (DataRow item in dt.Rows)
            {
                ListPost listPost = new();
                listPost.Name = (string)item[0];
                listPost.Message = (string)item[1];
                try
                {
                    MemoryStream ms = new((byte[])item[2]);
                    Image img = Image.FromStream(ms);
                    if (img.Width >= 500 || img.Height >= 500)
                    {
                        Image reImg = new Images().Resize(new Bitmap(img), new Size(300, 150));
                        listPost.Image = new Images().ConvertToB64(reImg);
                    }
                    else
                        listPost.Image = Convert.ToBase64String((byte[])item[4]);
                }
                catch (Exception)
                { }
                Posts.Add(listPost);
            }
            //SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    ListPost listPost = new();
            //    listPost.Name = reader.GetString(0);
            //    listPost.Message = reader.GetString(1);
            //    try
            //    {
            //        MemoryStream ms = new((byte[])reader[2]);
            //        Image img = Image.FromStream(ms);
            //        if (img.Width >= 500 || img.Height >= 500)
            //        {
            //            Image reImg = new Images().Resize(new Bitmap(img), new Size(300, 150));
            //            listPost.Image = new Images().ConvertToB64(reImg);
            //        }
            //        else
            //            listPost.Image = Convert.ToBase64String((byte[])reader[4]);
            //    }
            //    catch (Exception)
            //    { }
            //    Posts.Add(listPost);
            //}
            con.Close();
            Posts.Reverse();
            return Posts;
        }

        public List<ListComment> GetComments(int tid)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("GetComments", con);
            List<ListComment> Comments = new();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", tid);
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            foreach (DataRow item in dt.Rows)
            {
                ListComment listComment = new()
                {
                    Name = (string)item[0],
                    Comment = (string)item[1],
                    Date = new PostDate().Idk((DateTime)item[3]),
                    CommentID = (int)item[2]
                };
                Comments.Add(listComment);
            }
            //SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    ListComment listComment = new()
            //    {
            //        Name = reader.GetString(0),
            //        Comment = reader.GetString(1),
            //        Date = new PostDate().Idk(reader.GetDateTime(3)),
            //        CommentID = reader.GetInt32(2)
            //    };
            //    Comments.Add(listComment);
            //}
            con.Close();
            Comments.Reverse();
            return Comments;
        }

        public List<int> CommentSentimentCount(List<ListComment> comments, int tid)
        {
            SqlConnection con = new(connectionString);
            List<int> CommentSentimentCount = new();
            foreach (var item in comments)
            {
                SqlCommand cmd = new("CountCommentSentiment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CID", item.CommentID);
                cmd.Parameters.AddWithValue("@TID", tid);
                con.Open();
                CommentSentimentCount.Add((int)cmd.ExecuteScalar());
                con.Close();
            }
            return CommentSentimentCount;
        }

        public List<bool> CommentSentiment(int uid, int tid)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("GetCommentSentiment", con);
            List<bool> CommentSentiment = new();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UID", uid);
            cmd.Parameters.AddWithValue("@TID", tid);
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            foreach (DataRow item in dt.Rows)
            {
                CommentSentiment.Add((bool)item[0]);
            }
            //SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    CommentSentiment.Add(reader.GetBoolean(0));
            //}
            con.Close();
            CommentSentiment.Reverse();
            return CommentSentiment;
        }

        public void PostComment(int uid, int tid, string comment)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("CreateComment", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tweetID", tid);
            cmd.Parameters.AddWithValue("@userID", uid);
            cmd.Parameters.AddWithValue("@comment", comment);
            con.Open();
            int CID = (int)cmd.ExecuteScalar();
            con.Close();
            List<int> Users = new();
            cmd = new("GetAllUserIDs", con);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new();
            adapter.Fill(dt);
            foreach (DataRow item in dt.Rows)
            {
                Users.Add((int)item[0]);
            }
            //SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    Users.Add(reader.GetInt32(0));
            //}
            con.Close();
            foreach (var item in Users)
            {
                cmd = new("DefaultCommentSentiment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UID", item);
                cmd.Parameters.AddWithValue("@CID", CID);
                cmd.Parameters.AddWithValue("@TID", tid);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void SetCommentSentiment(int uid, int cid)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("UserCommentSentiment", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("UID", uid);
            cmd.Parameters.AddWithValue("CID", cid);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        #endregion Comment

        #region Login

        public int Login(string email, string password)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("UserLogin", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            foreach (DataRow item in dt.Rows)
            {
                if (Hash_Salt.PasswordAreEqual(password, (string)item["Password"], (string)item["Salt"]))
                {
                    int id = (int)item[0];
                    con.Close();
                    return id;
                }
            }
            //SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    if (Hash_Salt.PasswordAreEqual(password, reader.GetString("Password"), reader.GetString("Salt")))
            //    {
            //        int id = reader.GetInt32(0);
            //        con.Close();
            //        return id;
            //    }
            //}
            con.Close();
            return 0;
        }

        #endregion Login



        #region Signup

        public int Signup(string email, string password, string name)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("UserSignup", con);
            cmd.CommandType = CommandType.StoredProcedure;
            string salt = Hash_Salt.CreateSalt(16);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", Hash_Salt.GenerateHash(password, salt));
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@Salt", salt);
            int ID = 0;
            try
            {
                con.Open();
                ID = (int)cmd.ExecuteScalar();
                con.Close();
                cmd = new("UpdateFollowerID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FID", ID);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return ID;
            }
            catch (NullReferenceException)
            {
                return ID;
            }
        }

        public void DefaultSentiment(int id)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("GetTweets", con);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            foreach (DataRow item in dt.Rows)
            {
                SqlCommand cmd1 = new("DefaultSentiment", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@UID", id);
                cmd1.Parameters.AddWithValue("@TID", (int)item[2]);
                cmd1.ExecuteNonQuery();
            }

            //SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    SqlCommand cmd1 = new("DefaultSentiment", con);
            //    cmd1.CommandType = CommandType.StoredProcedure;
            //    cmd1.Parameters.AddWithValue("@UID", id);
            //    cmd1.Parameters.AddWithValue("@TID", reader.GetInt32(2));
            //    cmd1.ExecuteNonQuery();
            //}
            con.Close();
        }

        public void DefaultCommentSentiment(int id)
        {
            SqlConnection con = new(connectionString);
            List<ListPost> tweets = GetAllTweets();
            foreach (var item in tweets)
            {
                SqlCommand cmd = new("GetComment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TID", item.TweetID);
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    SqlCommand cmd1 = new("DefaultCommentSentiment", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@TID", item.TweetID);
                    cmd1.Parameters.AddWithValue("@CID", row[0]);
                    cmd1.Parameters.AddWithValue("@UID", id);
                    cmd1.ExecuteNonQuery();
                }
                //SqlDataReader reader = cmd.ExecuteReader();
                //while (reader.Read())
                //{
                //    SqlCommand cmd1 = new("DefaultCommentSentiment", con);
                //    cmd1.CommandType = CommandType.StoredProcedure;
                //    cmd1.Parameters.AddWithValue("@TID", item.TweetID);
                //    cmd1.Parameters.AddWithValue("@CID", reader.GetInt32(0));
                //    cmd1.Parameters.AddWithValue("@UID", id);
                //    cmd1.ExecuteNonQuery();
                //}
                con.Close();
            }
        }

        #endregion Signup

        #region Profile

        public ListProfile GetProfile(int id)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("GetUser", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                ListProfile listProfiles = new();
                listProfiles.Name = (string)row["Name"];
                try
                {
                    Images images = new();
                    listProfiles.PImg = images.ConvertToImage((byte[])row["ProfileImg"]);
                    listProfiles.HImg = images.ConvertToImage((byte[])row["HeaderImg"]);
                    listProfiles.Bio = (string)row["Bio"];
                }
                catch (Exception)
                {
                }
                return listProfiles;
            }
            //SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    ListProfile listProfiles = new();
            //    listProfiles.Name = reader.GetString("Name");
            //    try
            //    {
            //        Images images = new();
            //        listProfiles.PImg = images.ConvertToImage((byte[])reader["ProfileImg"]);
            //        listProfiles.HImg = images.ConvertToImage((byte[])reader["HeaderImg"]);
            //        listProfiles.Bio = reader.GetString("Bio");
            //    }
            //    catch (Exception)
            //    {
            //    }
            //    return listProfiles;
            //}
            con.Close();
            return null;
        }

        public int CountTweets(int id)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("CountTweets", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UID", id);
            con.Open();
            int TweetsCount = (int)cmd.ExecuteScalar();
            con.Close();
            return TweetsCount;
        }

        public void EditProfile(int id, IFormFile hImg, IFormFile pImg, string bio)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("EditProfile", con);
            cmd.CommandType = CommandType.StoredProcedure;
            byte[] headerBytes = new Images().ConvertToBytes(hImg);
            byte[] profileBytes = new Images().ConvertToBytes(pImg);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@Profile", profileBytes);
            cmd.Parameters.AddWithValue("@Header", headerBytes);
            cmd.Parameters.AddWithValue("@Bio", bio);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void Follow(int uid, int pid)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("Follow", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UID", uid);
            cmd.Parameters.AddWithValue("@PID", pid);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        #endregion Profile

        #region Message

        public List<Profiles> GetFollowers(int uid)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("GetFollowers", con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UID", uid);
            List<Profiles> profiles = new();
            con.Open();
            DataTable dt = new();
            adapter.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                ListProfile listProfile = GetProfile((int)row[2]);
                Profiles tItem = new();
                tItem.Name = listProfile.Name;
                tItem.id = (int)row[2];
                tItem.Img = new Images().ConvertToB64(new Images().Resize(listProfile.PImg, new Size(50, 50)));
                tItem.date = "Not Now";
                profiles.Add(tItem);
            }
            //SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    ListProfile listProfile = GetProfile(reader.GetInt32(2));
            //    Profiles tItem = new();
            //    tItem.Name = listProfile.Name;
            //    tItem.id = reader.GetInt32(2);
            //    tItem.Img = new Images().ConvertToB64(new Images().Resize(listProfile.PImg, new Size(50, 50)));
            //    tItem.date = "Not Now";
            //    profiles.Add(tItem);
            //}
            con.Close();
            return profiles;
        }

        public void CreateMessage(int uid, int fid, string message)
        {
            try
            {
                SqlConnection con = new(connectionString);
                SqlCommand cmd = new("CreateMessage", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UID", uid);
                cmd.Parameters.AddWithValue("@FID", fid);
                cmd.Parameters.AddWithValue("@Message", message);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<ListMessages> GetMessages(int uid, int fid)
        {
            List<ListMessages> messages = new();
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("GetMessages", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UID", uid);
            cmd.Parameters.AddWithValue("@FID", fid);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ListMessages messagesItem = new ListMessages()
                {
                    UserID = reader.GetInt32("UID"),
                    FollowerID = reader.GetInt32("FID"),
                    Message = reader.GetString("Message"),
                    Date = reader.GetDateTime("Date")
                };
                messages.Add(messagesItem);
            }
            con.Close();
            return messages;
        }

        public int GetFollow(int uid, int mid)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("GetFollow", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UID", uid);
            cmd.Parameters.AddWithValue("@MID", mid);
            int fid = 0;
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                fid = reader.GetInt32("FID");
            }
            con.Close();
            return fid;
        }

        public int? GetRoomID(int uid, int fid)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("GetRoomID", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UID", uid);
            cmd.Parameters.AddWithValue("@FID", fid);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            int? roomID = 0;
            while (reader.Read())
            {
                roomID = reader.GetInt32("ID");
            }
            con.Close();
            return roomID;
        }

        public void CreateRoom(int uid, int fid)
        {
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new("GetAllFollowers", con);
            cmd.CommandType = CommandType.StoredProcedure;
            bool Exist = true;
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader.GetInt32("UID") == uid && reader.GetInt32("FID") == fid || reader.GetInt32("FID") == uid && reader.GetInt32("UID") == fid)
                {
                    Exist = true;
                }
            }
            con.Close();
            if (Exist)
            {
                con.Open();
                cmd = new("CreateRoom", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UID", uid);
                cmd.Parameters.AddWithValue("@FID", fid);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        #endregion Message

        #region Notification
        public List<ListMessages> GetNotifications(List<Profiles> profiles, int id)
        {
            SqlConnection con = new(connectionString);
            List<ListMessages> list = new();
            foreach (var item in profiles)
            {
                SqlCommand cmd = new("GetNotifications", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@uid", item.id);
                cmd.Parameters.AddWithValue("@fid", id);
                con.Open();
                List<ListMessages> notification = new();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int temp = reader.GetInt32("UID");
                    if (reader.GetInt32("UID") != id)
                    {
                        ListMessages listMessages = new ListMessages()
                        {
                            UserID = reader.GetInt32("UID"),
                            Message = reader.GetString("Message"),
                            Date = reader.GetDateTime("Date")
                        };
                        notification.Add(listMessages);
                    }
                }
                con.Close();
                try
                {
                    notification.Sort((x, y) => DateTime.Compare(y.Date, x.Date));
                    list.Add(notification[0]);
                }
                catch (Exception)
                {
                }
            }
            return list;
        }

        public void SetRead(int userID, int followerID)
        {

        }
        #endregion
    }
}