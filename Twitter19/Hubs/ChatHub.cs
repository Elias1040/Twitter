using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twitter19.Repo;

namespace Twitter19.Hubs
{
    public class ChatHub : Hub
    {
        #region privateReadonly
        private readonly IRepo _repo;
        public ChatHub(IRepo repo)
        {
            _repo = repo;
        }
        #endregion

        public async Task SendMessage(int userID, int followerID, string message)
        {
            _repo.CreateMessage(userID, followerID, message);
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
