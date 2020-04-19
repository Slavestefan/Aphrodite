using System.Threading.Tasks;
using Discord.WebSocket;

namespace Slavestefan.Aphrodite.Web.MessageHandler
{
    public interface IDiscordMessageHandler
    {
        bool WantsToHandle(SocketMessage message);

        Task<bool> Handle(SocketMessage message);
    }
}