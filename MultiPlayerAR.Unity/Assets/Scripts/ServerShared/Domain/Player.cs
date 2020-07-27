using net.caffeineinject.multiplayerar.servershared.messagepackobjects;
using UnityEngine;

namespace net.caffeineinject.multiplayerar.domain
{
    public class Player
    {
        public string PlayerId { get; }
        public string PlayerName { get; }
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }

        private readonly DomainEventPublisher _domainEventPublisher;

        public Player(string playerId, string playerName, DomainEventPublisher domainEventPublisher)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            _domainEventPublisher = domainEventPublisher;
        }

        public void Move(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}