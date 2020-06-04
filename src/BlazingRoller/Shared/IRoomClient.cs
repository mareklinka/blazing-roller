using System;
using System.Threading.Tasks;

namespace BlazingRoller.Shared
{
    public interface IRoomClient
    {
        Task ReceiveRoll(string username, DiceThrowConfiguration config);

        Task ReceiveDicePositions(Guid throwId, DieFinalConfigurationWrapper config);
    }
}
