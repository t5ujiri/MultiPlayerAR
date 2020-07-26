using System.Collections.Generic;
using System.Linq;
using net.caffeineinject.multiplayerar.servershared.messagepackobjects;

namespace net.caffeineinject.multiplayerar.application
{
    public class EventStore
    {
        private readonly List<IEvent> _events = new List<IEvent>();

        public void AppendToStream(IEnumerable<IEvent> events)
        {
            _events.AddRange(events);
        }

        public EventStream LoadEventStream()
        {
            return new EventStream()
            {
                Events = _events.ToList()
            };
        }
    }
}