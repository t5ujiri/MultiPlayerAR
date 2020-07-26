using System;
using net.caffeineinject.multiplayerar.domain;
using net.caffeineinject.multiplayerar.servershared.messagepackobjects;
using UniRx;

namespace net.caffeineinject.multiplayerar.application
{
    public class ARWorldApplication : IDisposable
    {
        public EventStore EventStore { get; }
        private readonly string _roomName;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        public DomainEventPublisher DomainEventPublisher { get; }

        public ARWorldApplication(string roomName, EventStore eventStore, DomainEventPublisher domainEventPublisher)
        {
            _roomName = roomName;
            EventStore = eventStore;
            DomainEventPublisher = domainEventPublisher;
            DomainEventPublisher.EventStream.OfType<IEvent, PlayerJoined>()
                .Subscribe(e => { EventStore.AppendToStream(new IEvent[] {e}); })
                .AddTo(_compositeDisposable);
            DomainEventPublisher.EventStream.OfType<IEvent, PlayerSpoke>()
                .Subscribe(e => { EventStore.AppendToStream(new IEvent[] {e}); })
                .AddTo(_compositeDisposable);
            DomainEventPublisher.EventStream.OfType<IEvent, PlayerLeft>()
                .Subscribe(e => { EventStore.AppendToStream(new IEvent[] {e}); })
                .AddTo(_compositeDisposable);
        }

        public void Execute(ICommand cmd)
        {
            Console.WriteLine(cmd.ToString());
            // When((dynamic) cmd);
            switch (cmd)
            {
                case JoinCommand joinCommand:
                    When(joinCommand);
                    break;
                case PlayerSayCommand playerSayCommand:
                    When(playerSayCommand);
                    break;
                case LeaveCommand leaveCommand:
                    When(leaveCommand);
                    break;
            }
        }

        private void When(JoinCommand cmd)
        {
            Update(_roomName, world => world.AddPlayer(cmd.PlayerId, cmd.PlayerName));
        }

        private void When(PlayerSayCommand cmd)
        {
            Update(_roomName, world => world.Say(cmd.PlayerId, world.Players[cmd.PlayerId].PlayerName, cmd.Message));
        }

        private void When(LeaveCommand cmd)
        {
            Update(_roomName, world => world.RemovePlayer(cmd.PlayerId));
        }

        private void Update(string roomName, Action<ARWorld> execute)
        {
            var arWorld = new ARWorld(roomName, EventStore.LoadEventStream(), DomainEventPublisher);
            execute(arWorld);
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}