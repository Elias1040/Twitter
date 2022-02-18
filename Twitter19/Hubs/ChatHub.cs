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

        public async Task SendMessage(int userID, int followerID, string message, string roomID, bool join)
        {
            if (!join)
            {
                await JoinRoom(roomID).ConfigureAwait(false);
                try
                {
                    _repo.CreateMessage(userID, followerID, message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                await Clients.Group(roomID).SendAsync("ReceiveMessage", message, join);
            }
            else
                await Clients.Group(roomID.ToString()).SendAsync("ReceiveMessage", message, join);

        }

        public async Task JoinRoom(string roomID)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomID);
        }

        public async Task LeaveRoom(string roomID)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomID);
        }
    }
}
