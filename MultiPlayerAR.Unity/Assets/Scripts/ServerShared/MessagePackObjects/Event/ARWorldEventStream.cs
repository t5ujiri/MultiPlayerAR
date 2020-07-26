using MessagePack;

namespace net.caffeineinject.multiplayerar.servershared.messagepackobjects
{
    [MessagePackObject()]
    public class ARWorldEventStream : IEvent
    {
        [Key(0)] public string RoomName { get; set; }
        [Key(1)] public EventStream EventStream { get; set; }
    }
}