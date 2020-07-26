using System.Threading.Tasks;
using MagicOnion;
using net.caffeineinject.multiplayerar.servershared.messagepackobjects;

namespace net.caffeineinject.multiplayerar.application
{
    public interface IARWorldHubReceiver
    {
        void OnEvent(IEvent @event);
    }

    public interface IARWorldHub : IStreamingHub<IARWorldHub, IARWorldHubReceiver>
    {
        Task ExecuteAsync(ICommand cmd);
    }
}