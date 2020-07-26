using MessagePack;

namespace net.caffeineinject.multiplayerar.servershared.messagepackobjects
{
    [MessagePackObject()]
    public class PlayerLeft : IEvent
    {
        [Key(0)] public string PlayerId { get; set; }
        [Key(1)] public string PlayerName { get; set; }
    }
}