using System;
namespace BlazingRoller.Data.Model
{
    public class Room : EntityBase
    {
        public Guid RoomId { get; set; }

        public Guid RoomKey { get; set; }

        public string Name { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public int DerivationCycles { get; set; }

        public DateTime LastAction { get; set; }
    }
}
