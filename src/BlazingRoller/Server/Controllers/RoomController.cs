using System.Threading;
using System;
using Microsoft.AspNetCore.Mvc;
using BlazingRoller.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BlazingRoller.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : Controller
    {
        [Route("{id}/name")]
        public async Task<IActionResult> Get(Guid id, [FromServices] DataContext db, CancellationToken cancellationToken)
        {
            var roomName = await db.Rooms.Where(_ => _.RoomId == id).Select(_ => _.Name).SingleOrDefaultAsync(cancellationToken);

            if (roomName is null)
            {
                return NotFound();
            }

            return Json(roomName);
        }
    }
}