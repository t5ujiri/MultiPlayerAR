using MessagePack;

namespace net.caffeineinject.multiplayerar.servershared.messagepackobjects
{
    [MessagePackObject()]
    public class PlayerSpoke : IEvent
    {
        [Key(0)] public string PlayerId { get; set; }
        [Key(1)] public string PlayerName { get; set; }
        [Key(2)] public string Message { get; set; }
    }
}