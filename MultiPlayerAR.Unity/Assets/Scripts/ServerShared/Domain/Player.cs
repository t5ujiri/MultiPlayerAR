using net.caffeineinject.multiplayerar.servershared.messagepackobjects;
using UnityEngine;

namespace net.caffeineinject.multiplayerar.domain
{
    public class Player
    {
        public string PlayerId { get; }
        public string PlayerName { get; }
        private readonly DomainEventPublisher _domainEventPublisher;

        public Player(string playerId, string playerName, DomainEventPublisher domainEventPublisher)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            _domainEventPublisher = domainEventPublisher;
        }
        //
        // public void Mutate(IEvent @event)
        // {
        //     // When((dynamic) this);
        //     switch (@event)
        //     {
        //         case PlayerSpoke playerSpoke:
        //             When(playerSpoke);
        //             break;
        //     }
        // }
        //
        // private void When(PlayerSpoke playerSpoke)
        // {
        //     
        // }
    }
}