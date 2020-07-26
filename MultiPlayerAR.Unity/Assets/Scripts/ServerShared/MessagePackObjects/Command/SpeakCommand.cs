using MessagePack;

namespace net.caffeineinject.multiplayerar.servershared.messagepackobjects
{
    [MessagePackObject()]
    public class PlayerSayCommand : ICommand
    {
        [Key(0)] public string PlayerId { get; set; }
        [Key(1)] public string Message { get; set; }

        public override string ToString()
        {
            return $"{nameof(PlayerSayCommand)} PlayerId:{PlayerId} Message:{Message}";
        }
    }
}