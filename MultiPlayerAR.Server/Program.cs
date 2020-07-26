using System.Collections.Concurrent;
using System.Threading.Tasks;
using Grpc.Core;
using MagicOnion.Hosting;
using MagicOnion.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using net.caffeineinject.multiplayerar.application;

namespace MultiPlayerAR.Server
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            GrpcEnvironment.SetLogger(new Grpc.Core.Logging.ConsoleLogger());

            await MagicOnionHost.CreateDefaultBuilder()
                .UseMagicOnion(
                    new MagicOnionOptions(isReturnExceptionStackTraceInErrorDetail: true),
                    new ServerPort("0.0.0.0", 12345, ServerCredentials.Insecure))
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<ConcurrentDictionary<string, ARWorldApplication>>();
                })
                .RunConsoleAsync();
        }
    }
}