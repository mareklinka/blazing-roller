using System.Collections.Generic;
using System;
using BlazingRoller.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazingRoller.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private static Dictionary<string, (Guid Id, Guid Key, string Password)> _rooms = new Dictionary<string, (Guid Id, Guid Key, string Password)>();

        public IActionResult Post(LoginModel model)
        {
            var response = new LoginResponse();

            if (_rooms.TryGetValue(model.RoomName, out var x))
            {
                if (x.Password != model.RoomPassword)
                {
                    return Unauthorized();
                }

                response.RoomId = x.Id;
                response.RoomKey = x.Key;
            }
            else
            {
                response.RoomId = Guid.NewGuid();
                response.RoomKey = Guid.NewGuid();

                _rooms.Add(model.RoomName, (response.RoomId, response.RoomKey, model.RoomPassword));
            }

            return Ok(response);
        }
    }
}