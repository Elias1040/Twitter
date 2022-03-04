using Microsoft.AspNetCore.SignalR;
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

        #endregion privateReadonly

        public async Task SendMessageToGroup(int userID, int followerID, string message, bool join)
        {
            if (message != string.Empty)
            {
                if (!join)
                {
                    await JoinRoom(userID).ConfigureAwait(false);
                    _repo.CreateMessage(userID, followerID, message);
                    await Clients.Groups(followerID.ToString(), userID.ToString()).SendAsync("ReceiveMessage", message, join, userID, followerID);
                }
                else
                {
                    _repo.CreateMessage(userID, followerID, message);
                    await Clients.Groups(followerID.ToString(), userID.ToString()).SendAsync("ReceiveMessage", message, join, userID, followerID);
                }
            }
        }

        public async Task JoinRoom(int roomID)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomID.ToString());
            await base.OnConnectedAsync();
        }

        public async Task LeaveRoom(int roomID)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomID.ToString());
        }

        public void SetRead(int userID, int followerID)
        {
            _repo.SetRead(userID, followerID);
        }
    }
}