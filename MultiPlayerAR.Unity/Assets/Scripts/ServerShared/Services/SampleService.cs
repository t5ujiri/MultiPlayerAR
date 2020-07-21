using MagicOnion;

namespace net.caffeineinject.multiplayerar.servershared.services
{
    public interface ISampleService : IService<ISampleService>
    {
        UnaryResult<int> SumAsync(int x, int y);
        UnaryResult<int> ProductAsync(int x, int y);
    }
}