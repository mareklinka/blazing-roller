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
                var salt = new byte[16];
                RandomNumberGenerator.Fill(salt);

                var key = KeyDerivation.Pbkdf2(model.RoomPassword,
                                               salt,
                                               KeyDerivationPrf.HMACSHA256,
                                               Constants.PasswordIterations,
                                               Constants.KeyLength);

                var room = new Room
                {
                    Name = model.RoomName,
                    RoomId = Guid.NewGuid(),
                    RoomKey = Guid.NewGuid(),
                    DerivationCycles = Constants.PasswordIterations,
                    LastAction = DateTime.Now,
                    PasswordHash = key,
                    PasswordSalt = salt
                };

                response.RoomId = room.RoomId;
                response.RoomKey = room.RoomKey;

                db.Rooms.Add(room);

                await db.SaveChangesAsync(cancellationToken);
            }
            else
            {
                var key = KeyDerivation.Pbkdf2(model.RoomPassword,
                                               existingRoom.PasswordSalt,
                                               KeyDerivationPrf.HMACSHA256,
                                               Constants.PasswordIterations,
                                               Constants.KeyLength);

                if (!key.SequenceEqual(existingRoom.PasswordHash))
                {
                    return Unauthorized();
                }

                response.RoomId = existingRoom.RoomId;
                response.RoomKey = existingRoom.RoomKey;

                existingRoom.LastAction = DateTime.Now;
                await db.SaveChangesAsync(cancellationToken);
            }

            await t.CommitAsync(cancellationToken);

            return Ok(response);
        }
    }
}