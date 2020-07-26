using MessagePack;

namespace net.caffeineinject.multiplayerar.servershared.messagepackobjects
{
    [Union(0, typeof(PlayerJoined))]
    [Union(1, typeof(PlayerLeft))]
    [Union(2, typeof(PlayerSpoke))]
    [Union(3, typeof(PlayerMoved))]
    [Union(4, typeof(ARWorldEventStream))]
    public interface IEvent
    {
    }
}