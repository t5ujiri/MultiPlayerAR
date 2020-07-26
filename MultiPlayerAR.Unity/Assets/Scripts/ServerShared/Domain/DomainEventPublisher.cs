using net.caffeineinject.multiplayerar.servershared.messagepackobjects;
using UniRx;

namespace net.caffeineinject.multiplayerar.domain
{
    public class DomainEventPublisher
    {
        public readonly Subject<IEvent> EventStream = new Subject<IEvent>();

        public void Publish(IEvent @event)
        {
            EventStream.OnNext(@event);
        }
    }
}