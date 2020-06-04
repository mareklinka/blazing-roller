using System.Threading;
using System.Security.Cryptography;
using System;
using BlazingRoller.Shared;
using Microsoft.AspNetCore.Mvc;
using BlazingRoller.Data;
using System.Linq;
using BlazingRoller.Data.Model;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BlazingRoller.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        public async Task<IActionResult> Post(LoginModel model, [FromServices] DataContext db, CancellationToken cancellationToken)
        {
            var response = new LoginResponse();

            await using var t = await db.Database.BeginTransactionAsync(cancellationToken);

            var existingRoom = await db.Rooms.SingleOrDefaultAsync(_ => _.Name == model.RoomName);

            if (existingRoom is null)
            {
                existingRoom = new Room { Name = model.RoomName };
                ConfigureRoom(existingRoom, model.RoomPassword);

                db.Rooms.Add(existingRoom);
            }
            else
            {
                if (existingRoom.LastAction.AddHours(12) < DateTime.Now)
                {
                    // room has been inactive for more than 12 hours - allow reuse
                    ConfigureRoom(existingRoom, model.RoomPassword);
                }
                else
                {
                    // active room - password must match
                    var key = KeyDerivation.Pbkdf2(model.RoomPassword,
                                               existingRoom.PasswordSalt,
                                               KeyDerivationPrf.HMACSHA256,
                                               Constants.PasswordIterations,
                                               Constants.KeyLength);

                    if (!key.SequenceEqual(existingRoom.PasswordHash))
                    {
                        return Unauthorized();
                    }
                }
            }

            existingRoom.LastAction = DateTime.Now;

            await db.SaveChangesAsync(cancellationToken);
            await t.CommitAsync(cancellationToken);

            response.RoomId = existingRoom.RoomId;
            response.RoomKey = existingRoom.RoomKey;

            return Ok(response);
        }

        private void ConfigureRoom(Room room, string password)
        {
            var salt = new byte[16];
            RandomNumberGenerator.Fill(salt);

            var key = KeyDerivation.Pbkdf2(password,
                                           salt,
                                           KeyDerivationPrf.HMACSHA256,
                                           Constants.PasswordIterations,
                                           Constants.KeyLength);

            room.RoomId = Guid.NewGuid();
            room.RoomKey = Guid.NewGuid();
            room.DerivationCycles = Constants.PasswordIterations;
            room.LastAction = DateTime.Now;
            room.PasswordHash = key;
            room.PasswordSalt = salt;
        }
    }
}