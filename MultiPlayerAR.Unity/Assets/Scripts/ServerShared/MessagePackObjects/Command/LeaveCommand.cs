using MessagePack;

namespace net.caffeineinject.multiplayerar.servershared.messagepackobjects
{
    [MessagePackObject()]
    public class LeaveCommand : ICommand
    {
        [Key(0)] public string PlayerId { get; set; }

        public override string ToString()
        {
            return $"{nameof(LeaveCommand)} PlayerId:{PlayerId}";
        }
    }
}