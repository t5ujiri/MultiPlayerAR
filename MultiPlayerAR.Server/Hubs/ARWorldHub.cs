using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MagicOnion.Server.Hubs;
using net.caffeineinject.multiplayerar.application;
using net.caffeineinject.multiplayerar.domain;
using net.caffeineinject.multiplayerar.servershared.messagepackobjects;
using UniRx;

namespace net.caffeineinject.multiplayerar.server.hubs
{
    /// <summary>
    /// インターフェースと認証
    /// </summary>
    public class ARWorldHub : StreamingHubBase<IARWorldHub, IARWorldHubReceiver>, IARWorldHub, IDisposable
    {
        private IGroup _group;
        private readonly ConcurrentDictionary<string, ARWorldApplication> _applications;
        private string _roomName;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public ARWorldHub(ConcurrentDictionary<string, ARWorldApplication> applications)
        {
            _applications = applications;
        }

        public async Task ExecuteAsync(ICommand cmd)
        {
            await this.When((dynamic) cmd);
        }

        private async Task When(JoinCommand cmd)
        {
            _group = await Group.AddAsync(cmd.RoomName);
            var application = _applications.GetOrAdd(cmd.RoomName,
                newRoomName => new ARWorldApplication(newRoomName, new EventStore(), new DomainEventPublisher()));
            _roomName = cmd.RoomName;

            application.DomainEventPublisher.EventStream
                .Subscribe(e =>
                {
                    switch (e)
                    {
                        case PlayerJoined joined:
                            BroadcastToSelf(_group).OnEvent(joined);
                            Console.WriteLine($"joined: {joined.PlayerId} as {joined.PlayerName} to {joined.RoomName}");
                            break;
                        case PlayerLeft left:
                            BroadcastToSelf(_group).OnEvent(left);
                            Console.WriteLine($"left: {left.PlayerId}");
                            break;
                        case PlayerSpoke playerSpoke:
                            BroadcastToSelf(_group).OnEvent(playerSpoke);
                            Console.WriteLine($"spoke: {playerSpoke.PlayerId} like {playerSpoke.Message}");
                            break;
                        case PlayerMoved playerMoved:
                            BroadcastToSelf(_group).OnEvent(playerMoved);
                            // Console.WriteLine($"spoke: {playerSpoke.PlayerId} like {playerSpoke.Message}");
                            break;
                    }
                }).AddTo(_disposables);

            application.Execute(cmd);

            BroadcastToSelf(_group).OnEvent(
                new ARWorldEventStream()
                {
                    RoomName = _roomName,
                    EventStream = application.EventStore.LoadEventStream()
                });
        }

        private async Task When(LeaveCommand cmd)
        {
            _applications[_roomName].Execute(cmd);
            await _group.RemoveAsync(Context);
            _disposables.Dispose();
        }

        private Task When(ICommand cmd)
        {
            _applications[_roomName].Execute(cmd);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}