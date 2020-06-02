using System.Threading.Tasks;

namespace BlazingRoller.Shared
{
    public interface IRoomClient
    {
        Task ReceiveRoll(string sample);
    }
}
