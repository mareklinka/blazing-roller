using System;
namespace BlazingRoller.Client.Services
{
    public class SessionStorageService
    {
        public string User { get; set; }

        public string Room { get; set; }

        public Guid RoomKey { get; set; }
    }
}