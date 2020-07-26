using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using net.caffeineinject.multiplayerar.domain;
using net.caffeineinject.multiplayerar.servershared.messagepackobjects;
using UniRx;
using UnityEngine;

namespace Client.Application.Service
{
    public class TestController : MonoBehaviour
    {
        private async void Start()
        {
            var client = new ARWorldClient();
            var events = new List<IEvent>();
            var domainEventPublisher = new DomainEventPublisher();
            var arWorld = new ARWorld("default", new EventStream() {Events = new List<IEvent>()}, domainEventPublisher);

            client.OnEventReceived.Subscribe(e =>
            {
                if (e is ARWorldEventStream arWorldEventStream)
                {
                    events = arWorldEventStream.EventStream.Events;
                    arWorld = new ARWorld("default", arWorldEventStream.EventStream, domainEventPublisher);
                }
                else
                {
                    events.Add(e);
                    arWorld.Mutate(e);
                }

                Debug.Log(e.GetType().ToString());
            });

            client.Connect();
            var playerId = Guid.NewGuid().ToString();

            await client.ExecuteAsync(
                new JoinCommand()
                {
                    PlayerId = playerId,
                    Position = Vector3.zero,
                    RoomName = "default",
                    Rotation = Quaternion.identity,
                    PlayerName = "test"
                });

            await UniTask.Delay(TimeSpan.FromSeconds(3));
            await client.ExecuteAsync(
                new PlayerSayCommand()
                {
                    Message = "Hello World!",
                    PlayerId = playerId
                });
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            await client.ExecuteAsync(new LeaveCommand()
            {
                PlayerId = playerId
            });
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            await client.WaitForDisconnect();
            await client.DisposeAsync();
        }
    }
}