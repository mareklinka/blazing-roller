using System;
using System.ComponentModel.DataAnnotations;

namespace BlazingRoller.Shared
{
    public class LoginResponse
    {
        public Guid RoomId { get; set; }

        public Guid RoomKey { get; set; }
    }
}
