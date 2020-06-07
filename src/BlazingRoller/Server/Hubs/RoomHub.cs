using System;
using System.Threading.Tasks;
using BlazingRoller.Data;
using BlazingRoller.Shared;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BlazingRoller.Server.Hubs
{
    public class RoomHub : Hub<IRoomClient>, IRoomHub
    {
        private readonly DataContext _db;

        public RoomHub(DataContext db) => _db = db;

        public Task JoinRoom()
        {
            var roomKey = GetRoomKey();
            return Groups.AddToGroupAsync(Context.ConnectionId, roomKey);
        }

        public async Task RollDice(string username, DiceThrowConfiguration config)
        {
            var roomId = GetRoomId();
            var roomKey = GetRoomKey();

            await UpdateRoomActiveDate(roomId);

            await Clients.OthersInGroup(roomKey).ReceiveRoll(username, config);
        }

        public async Task RepositionDice(Guid throwId, DieFinalConfigurationWrapper config)
        {
            var roomId = GetRoomId();
            var roomKey = GetRoomKey();

            await UpdateRoomActiveDate(roomId);

            await Clients.OthersInGroup(roomKey).ReceiveDicePositions(throwId, config);
        }

        private Guid GetRoomId() => Guid.Parse(Context.GetHttpContext().Request.Query["roomId"].ToString());
        private string GetRoomKey() => Context.GetHttpContext().Request.Query["roomKey"].ToString();

        private async Task UpdateRoomActiveDate(Guid roomId)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            var room = await _db.Rooms.SingleOrDefaultAsync(_ => _.RoomId == roomId);

            if (room is null)
            {
                return;
            }

            room.LastAction = DateTime.Now;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
    }
}
