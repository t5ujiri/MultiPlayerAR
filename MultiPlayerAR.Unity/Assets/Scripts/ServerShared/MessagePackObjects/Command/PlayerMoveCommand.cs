using MessagePack;
using UnityEngine;

namespace net.caffeineinject.multiplayerar.servershared.messagepackobjects
{
    [MessagePackObject()]
    public class PlayerMoveCommand : ICommand
    {
        [Key(0)] public string PlayerId { get; set; }
        [Key(1)] public Vector3 Position { get; set; }
        [Key(2)] public Quaternion Rotation { get; set; }

        public override string ToString()
        {
            return $"{nameof(PlayerMoveCommand)} PlayerId:{PlayerId} Position:{Position} Rotation:{Rotation}";
        }
    }
}