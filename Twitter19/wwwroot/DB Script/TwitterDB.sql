------------Database------------

USE [MASTER]
GO
-- !!!! Ignore on first creation of database !!!!
IF db_id('Twitter') is not null
begin
ALTER DATABASE [Twitter] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
DROP DATABASE [Twitter]
end
go

CREATE DATABASE [Twitter]
GO
USE [Twitter]
GO

------------Tables------------

CREATE TABLE [Users] (
  [ID] int primary key IDENTITY(1, 1),
  [Email] varchar(50) NOT NULL,
  [Password] varchar(50) NOT NULL,
  [Salt] varchar(50) NOT NULL,
  [Name] varchar(50) NOT NULL,
  [ProfileImg] varbinary(max),
  [HeaderImg] varbinary(max),
  [Bio] varchar(max)
)
GO

CREATE TABLE [Tweets] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [Tweet] varchar(50) NOT NULL,
  [Date] datetime NOT NULL,
  [UserID] int NOT NULL,
  [Image] varbinary(max)
  FOREIGN KEY ([UserID]) REFERENCES [Users] ([ID])
)
GO

CREATE TABLE [Comments] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [Comment] varchar(50) NOT NULL,
  [Date] datetime NOT NULL,
  [TweetID] int NOT NULL,
  [UserID] int NOT NULL
  FOREIGN KEY ([UserID]) REFERENCES [Users] ([ID]),
  FOREIGN KEY ([TweetID]) REFERENCES [Tweets] ([ID])
)
GO

CREATE TABLE [Sentiment] (
  [ID] int PRIMARY KEY NOT NULL IDENTITY(1, 1),
  [TweetID] int NOT NULL,
  [UserID] int NOT NULL,
  [Sentiment] bit NOT NULL
  FOREIGN KEY ([UserID]) REFERENCES [Users] ([ID]),
  FOREIGN KEY ([TweetID])	REFERENCES [Tweets] ([ID])
)
GO

CREATE TABLE [CommentSentiment] (
  [ID] int PRIMARY KEY NOT NULL IDENTITY(1, 1),
  [Sentiment] bit NOT NULL,
  [UserID] int NOT NULL,
  [TweetID] int NOT NULL,
  [CommentID] int NOT NULL
  FOREIGN KEY ([UserID]) REFERENCES [Users] ([ID]),
  FOREIGN KEY ([TweetID]) REFERENCES [Tweets] ([ID]),
  FOREIGN KEY ([CommentID]) REFERENCES [Comments] ([ID])
)
GO

create table [UserFollow](
  [FollowID] int primary key NOT NULL
  foreign key (FollowID) references Users(ID)
)
go

CREATE TABLE [Followers] (
  [ID] int identity(1,1) not null,
  [UID] int NOT NULL,
  [FID] int NOT NULL,
  [Follow] bit NOT NULL
  primary key(UID, FID)
  foreign key (UID) references Users(ID),
  foreign key (FID) references UserFollow(FollowID)
)
GO

CREATE TABLE [dbo].[Messages](
	[ID] int NOT NULL,
	[UID] int NOT NULL,
	[FID] int NOT NULL,
	[Message] varchar(max) NOT NULL,
	[Date] [datetime] NOT NULL
)
GO

alter table [Messages]
add foreign key (UID, FID) references Followers(UID, FID)
go




	------------SP Select------------

	CREATE procedure CreateComment
	@tweetID int, @userID int, @comment varchar(50)
	as
		declare @inserted table(ID int)
		set nocount on
		insert into Comments(TweetID, UserID, Date, Comment)
			output INSERTED.ID
			into @inserted
		values (@tweetID, @userID, getdate(), @comment)
		select ID from @inserted
	return
	GO

	CREATE procedure GetComments
	@id int
	as
		select U.Name, C.Comment, C.ID, C.Date from Users as U, Comments as C
		where C.TweetID = @id and U.ID = C.UserID
	GO
	
	CREATE procedure GetAllComments
	as
		select U.Name, C.Comment, C.ID, C.Date from Users as U, Comments as C
		where U.ID = C.UserID
	GO

	CREATE procedure GetSingleTweet
	@tweetID int
	as
		select U.Name, T.Tweet, T.Image, T.Date from Users as U, Tweets as T where T.ID = @tweetID
	GO

	CREATE procedure GetTweets
	as
		select U.Name, T.Tweet, T.ID, T.Date, T.Image from Users as U, Tweets as T
		where U.ID = T.UserID
	GO

	create procedure GetAllTweets
	as
		select ID from Tweets
	go

	create procedure GetUser
	@id int
	as
		select * from Users
		where ID = @id
	GO

	create procedure GetAllUserIDs
	as
		select ID from Users
	GO

	CREATE procedure UserLogin
	@Email varchar(50)
	as
		select * from Users
		where Email = @Email
	GO

	create procedure GetSentiment
	@UID int
	as
		select Sentiment from [Sentiment]
		where UserID = @UID
	GO

	create procedure GetCommentSentiment
	@UID int, @TID int
	as
		select Sentiment from [CommentSentiment]
		where UserID = @UID and TweetID = @TID
	GO

	create procedure GetComment
	@TID int
	as
		select ID from Comments
		where TweetID = @TID
	go

	create procedure CountSentiment
	@TID int
	as
		select count(Sentiment) from Sentiment
		where TweetID = @TID and Sentiment = 1
	go

	create procedure CountCommentSentiment
	@TID int, @CID int
	as
		select count(Sentiment) from CommentSentiment
		where TweetID = @TID and CommentID = @CID and Sentiment = 1
	go

	create procedure CountComments
	@TID int
	as
		select count(ID) from Comments
		where TweetID = @TID
	go

	create procedure CountTweets
	@UID int
	as
		select count(ID) from Tweets
		where UserID = @UID
	go

	create procedure GetFollowers
	@UID int
	as
		select * from Followers
		where UID = @UID
	go

	create procedure GetFollowersBySearch
	@FID int, @FollowerName varchar(50)
	as
		select * from Users
		where Name = @FollowerName and ID = @FID
	go

	create procedure GetFollowersByID
	@FID int
	as
		select * from Users
		where ID = @FID
	go

	create procedure GetMessages
	@UID int, @FID int
	as
		select * from [Messages]
		where UID = @UID and FID = @FID 
	go


	------------SP Create------------

	CREATE procedure UserSignup
	@Email varchar(50), @Password varchar(50), @Name varchar(50), @Salt varchar(50)
	as
		set nocount on
		IF EXISTS (SELECT Email FROM Users WHERE Email = @Email)
		begin
			return
		end
		else 
		begin
			declare @inserted table(ID int)
			insert into Users (Email, Password, Name, Salt)
				output INSERTED.ID
				into @inserted
			values (@Email, @Password, @Name, @Salt)
			select ID from @inserted
			return 
		end
	GO

	CREATE procedure CreateTweet
	@tweet varchar(50), @user int, @imagebytes varbinary(max) = null
	as
		declare @inserted table(ID int)
		set nocount on
		insert into Tweets(Tweet, UserID, Date, Image)
			output INSERTED.ID
			into @inserted
		values (@tweet, @user, getdate(), @imagebytes)
		select ID from @inserted
	return
	GO

	create procedure CreateMessage
	@UID int, @FID int, @Message varchar(max)
	as
		insert into [Messages] (UID, FID, Message, Date)
		values (@UID, @FID, @Message, getdate())
	go


	------------SP Update------------

	create procedure UpdateFollowerID
	@FID int
	as
		insert into UserFollow
		values (@FID)
	go

	CREATE procedure EditProfile
	@Header varbinary(max) = null, @Profile varbinary(max) = null, @Bio varchar(max) = null, @id int
	as
		update Users
			set HeaderImg = @Header, ProfileImg = @Profile, Bio = @Bio
		where ID = @id
	GO

	CREATE procedure DefaultSentiment
	@UID int, @TID int
	as
		insert into Sentiment(UserID, TweetID, Sentiment)
		values (@UID, @TID, 0)
	GO

	CREATE procedure DefaultCommentSentiment
	@UID int, @CID int, @TID int
	as
		insert into CommentSentiment(UserID, CommentID, TweetID, Sentiment)
		values (@UID, @CID, @TID, 0)
	GO


	------------SP Other------------
	CREATE procedure UserSentiment
	@UID int, @TID int
	as
		if exists (select * from [Sentiment] where TweetID = @TID and UserID = @UID)
		begin
			if (select Sentiment from [Sentiment] where TweetID = @TID and UserID = @UID) != 1
			begin
				update Sentiment
					set Sentiment = 1
				where TweetID = @TID and UserID = @UID
			end
			else if (select Sentiment from [Sentiment] where TweetID = @TID and UserID = @UID) = 1
			begin
				update Sentiment
					set Sentiment = 0
				where TweetID = @TID and UserID = @UID
			end
		end
		else
		begin
			insert into Sentiment(TweetID, UserID, Sentiment)
			values (@TID, @UID, 1)
		end
	GO

	CREATE procedure UserCommentSentiment
	@UID int, @CID int
	as
		if exists (select * from [CommentSentiment] where UserID = @UID and CommentID = @CID)
		begin
			if (select Sentiment from [CommentSentiment] where UserID = @UID and CommentID = @CID) != 1
			begin
				update CommentSentiment
					set Sentiment = 1
				where UserID = @UID and CommentID = @CID
			end
			else if (select Sentiment from [CommentSentiment] where UserID = @UID and CommentID = @CID) = 1
			begin
				update CommentSentiment
					set Sentiment = 0
				where UserID = @UID and CommentID = @CID
			end
		end
		else
		begin
			insert into CommentSentiment(UserID, CommentID, Sentiment)
			values (@UID, @CID, 1)
		end
	GO

	create procedure Follow
	@UID int, @PID int
	as
		if exists (select * from Followers where UID = @UID and FID = @PID)
		begin
			if (select Follow from Followers where UID = @UID and FID = @PID) = 1
			begin
				update Followers
					set Follow = 0
				where [UID] = @UID and FID = @PID
			end
			else
			begin
				update Followers
					set Follow = 1
				where [UID] = @UID and FID = @PID
			end
		end
		else
		begin
			insert into Followers
			values (@UID, @PID, 1)
		end
	go