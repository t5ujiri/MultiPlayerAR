using System.Collections.Generic;
using MessagePack;

namespace net.caffeineinject.multiplayerar.servershared.messagepackobjects
{
    [MessagePackObject()]
    public class EventStream
    {
        [Key(0)] public List<IEvent> Events { get; set; }
    }
}