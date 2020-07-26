using Cysharp.Threading.Tasks;
using Grpc.Core;
using MagicOnion.Client;
using net.caffeineinject.multiplayerar.application;
using net.caffeineinject.multiplayerar.servershared.messagepackobjects;
using UniRx;
using Channel = Grpc.Core.Channel;

namespace Client.Application.Service
{
    public class ARWorldClient : IARWorldHubReceiver
    {
        private IARWorldHub _client;
        private Channel _channel;

        public Subject<IEvent> OnEventReceived { get; } = new Subject<IEvent>();

        public void Connect()
        {
            _channel = new Channel("localhost", 12345, ChannelCredentials.Insecure);
            _client = StreamingHubClient.Connect<IARWorldHub, IARWorldHubReceiver>(_channel, this);
        }

        public async UniTask ExecuteAsync(ICommand cmd)
        {
            if (_client == null)
            {
                return;
            }

            await _client.ExecuteAsync(cmd);
        }

        // You can watch connection state, use this for retry etc.
        public UniTask WaitForDisconnect()
        {
            return _client.WaitForDisconnect().AsUniTask();
        }

        public async UniTask DisposeAsync()
        {
            await _client.DisposeAsync();
            await _channel.ShutdownAsync();
            _channel = null;
        }

        public void OnEvent(IEvent @event)
        {
            OnEventReceived.OnNext(@event);
        }
    }
}