using System.Threading;
using System;
using Microsoft.AspNetCore.Mvc;
using BlazingRoller.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BlazingRoller.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : Controller
    {
        [Route("{id}/name")]
        public async Task<IActionResult> Get(Guid id, [FromServices] DataContext db, CancellationToken cancellationToken)
        {
            var room = await db.Rooms.SingleOrDefaultAsync(_ => _.RoomId == id, cancellationToken);

            if (room is null)
            {
                return NotFound();
            }

            return Json(room.Name);
        }
    }
}