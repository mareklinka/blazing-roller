using System;
using System.Threading.Tasks;
using BlazingRoller.Shared;
using Microsoft.AspNetCore.SignalR;

namespace BlazingRoller.Server.Hubs
{
    public class RoomHub : Hub<IRoomClient>
    {
        public Task JoinRoom()
        {
            var roomKey = Context.GetHttpContext().Request.Query["roomKey"].ToString();
            return Groups.AddToGroupAsync(Context.ConnectionId, roomKey);
        }

        public async Task RollDice(int seed)
        {
            var roomKey = Context.GetHttpContext().Request.Query["roomKey"].ToString();
            await Clients.OthersInGroup(roomKey).ReceiveRoll(seed);
        }
    }
}