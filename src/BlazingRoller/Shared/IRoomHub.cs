using System;
using System.Threading.Tasks;

namespace BlazingRoller.Shared
{
    public interface IRoomHub
    {
        Task JoinRoom();

        Task RollDice(string username, DiceThrowConfiguration config);

        Task RepositionDice(Guid throwId, DieFinalConfigurationWrapper config);
    }
}
