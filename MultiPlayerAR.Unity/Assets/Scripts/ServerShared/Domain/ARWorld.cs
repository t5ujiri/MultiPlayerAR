using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using net.caffeineinject.multiplayerar.servershared.messagepackobjects;
using UnityEngine;

namespace net.caffeineinject.multiplayerar.domain
{
    public class ARWorld
    {
        public ConcurrentDictionary<string, Player> Players { get; } = new ConcurrentDictionary<string, Player>();
        public List<PlayerSpoke> Messages { get; set; } = new List<PlayerSpoke>();
        public string RoomName { get; }

        private readonly DomainEventPublisher _domainEventPublisher;

        public ARWorld(string roomName, EventStream eventStream, DomainEventPublisher domainEventPublisher)
        {
            _domainEventPublisher = domainEventPublisher;
            RoomName = roomName;
            foreach (var @event in eventStream.Events)
            {
                Mutate(@event);
            }
        }

        public void Mutate(IEvent @event)
        {
            switch (@event)
            {
                case PlayerJoined playerJoined:
                    When(playerJoined);
                    break;
                case PlayerLeft playerLeft:
                    When(playerLeft);
                    break;
                case PlayerSpoke playerSpoke:
                    When(playerSpoke);
                    break;
                case PlayerMoved playerMoved:
                    When(playerMoved);
                    break;
            }
        }

        private void When(PlayerJoined @event)
        {
            if (!Players.TryAdd(@event.PlayerId, new Player(@event.PlayerId, @event.PlayerName, _domainEventPublisher)))
            {
                Console.WriteLine($"player {@event.PlayerId} is already added");
            }
        }

        private void When(PlayerMoved playerMoved)
        {
            Players[playerMoved.PlayerId].Move(playerMoved.Position, playerMoved.Rotation);
        }

        private void When(PlayerSpoke @event)
        {
            Messages.Add(@event);
        }

        private void When(PlayerLeft @event)
        {
            if (!Players.TryRemove(@event.PlayerId, out var player))
            {
                Console.WriteLine($"player {@event.PlayerId} is already removed");
            }
        }

        public void AddPlayer(string playerId, string playerName)
        {
            _domainEventPublisher.Publish(new PlayerJoined(RoomName, playerId, playerName, new Vector3(),
                new Quaternion()));
        }

        public void RemovePlayer(string playerId)
        {
            _domainEventPublisher.Publish(new PlayerLeft()
            {
                PlayerId = playerId,
                PlayerName = Players[playerId].PlayerName
            });
        }

        public void Say(string playerId, string playerName, string message)
        {
            _domainEventPublisher.Publish(
                new PlayerSpoke()
                {
                    PlayerId = playerId,
                    PlayerName = playerName,
                    Message = message
                });
        }

        public void MovePlayer(string playerId, Vector3 moveCommandPosition, Quaternion moveCommandRotation)
        {
            _domainEventPublisher.Publish(
                new PlayerMoved()
                {
                    PlayerId = playerId,
                    Position = moveCommandPosition,
                    Rotation = moveCommandRotation
                });
        }
    }
}