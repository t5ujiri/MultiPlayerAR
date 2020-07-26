using MessagePack;
using UnityEngine;

namespace net.caffeineinject.multiplayerar.servershared.messagepackobjects
{
    [MessagePackObject()]
    public class JoinCommand : ICommand
    {
        [Key(0)] public string RoomName { get; set; }
        [Key(1)] public string PlayerId { get; set; }
        [Key(2)] public string PlayerName { get; set; }
        [Key(3)] public Vector3 Position { get; set; }
        [Key(4)] public Quaternion Rotation { get; set; }

        public override string ToString()
        {
            return $"{nameof(JoinCommand)} PlayerId:{PlayerId} PlayerName:{PlayerName}";
        }
    }
}