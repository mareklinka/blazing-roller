using System;
using System.ComponentModel.DataAnnotations;

namespace BlazingRoller.Shared
{
    public class LoginModel
    {
        [Required]
        [StringLength(40)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string RoomName { get; set; }

        [Required]
        public string RoomPassword { get; set; }
    }
}
