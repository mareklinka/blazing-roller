using System;
namespace BlazingRoller.Client.Services
{
    public class SessionStorageService
    {
        public string User { get; set; }

        public string Room { get; set; }

        public Guid RoomKey { get; set; }

        public void Clear()
        {
            User = null;
            Room = null;
            RoomKey = Guid.Empty;
        }
    }
}
