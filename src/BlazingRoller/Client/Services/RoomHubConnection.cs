using System;
using System.Threading.Tasks;
using BlazingRoller.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazingRoller.Client.Services
{
    public class RoomHubConnection : IRoomHub, IDisposable
    {
        private HubConnection _hubConnection;
        private readonly NavigationManager _navigator;

        public RoomHubConnection(NavigationManager navigator) => _navigator = navigator;

        public void BuildConnection(string roomId, string roomKey)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigator.ToAbsoluteUri($"/roomHub?roomKey={roomKey}&roomId={roomId}"))
                .WithAutomaticReconnect()
                .Build();
        }

        public Task Connect() => _hubConnection.StartAsync();

        public void Dispose()
        {
            if (_hubConnection is {})
            {
                _ = _hubConnection.DisposeAsync();
            }
        }

        public IDisposable OnReceiveRoll(Func<string, DiceThrowConfiguration, Task> handler) =>
            _hubConnection.On(nameof(IRoomClient.ReceiveRoll), handler);

        public IDisposable OnReceiveDicePositions(Func<Guid, DieFinalConfigurationWrapper, Task> handler) =>
            _hubConnection.On(nameof(IRoomClient.ReceiveDicePositions), handler);

        public Task JoinRoom() => _hubConnection.SendAsync(nameof(IRoomHub.JoinRoom));

        public Task RollDice(string username, DiceThrowConfiguration config) =>
            _hubConnection.SendAsync(nameof(IRoomHub.RollDice), username, config);

        public Task RepositionDice(Guid throwId, DieFinalConfigurationWrapper config) =>
            _hubConnection.SendAsync(nameof(IRoomHub.RepositionDice), throwId, config);
    }
}
