using MessagePack;

namespace net.caffeineinject.multiplayerar.servershared.messagepackobjects
{
    [Union(0, typeof(JoinCommand))]
    [Union(1, typeof(LeaveCommand))]
    [Union(2, typeof(PlayerSayCommand))]
    [Union(3, typeof(PlayerMoveCommand))]
    public interface ICommand
    {
    }
}