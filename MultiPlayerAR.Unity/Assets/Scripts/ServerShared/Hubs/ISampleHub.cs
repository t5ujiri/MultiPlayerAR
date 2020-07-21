using MagicOnion;
using System.Threading.Tasks;
using net.caffeineinject.multiplayerar.servershared.messagepackobjects;
using UnityEngine;

namespace net.caffeineinject.multiplayerar.servershared.hubs
{
    /// <summary>
    /// CLient -> ServerのAPI
    /// </summary>
    public interface ISampleHub : IStreamingHub<ISampleHub, ISampleHubReceiver>
    {
        /// <summary>
        /// ゲームに接続することをサーバに伝える
        /// </summary>
        Task JoinAsync(Player player);

        /// <summary>
        /// ゲームから切断することをサーバに伝える
        /// </summary>
        Task LeaveAsync();

        /// <summary>
        /// メッセージをサーバに伝える
        /// </summary>
        Task SendMessageAsync(string message);

        /// <summary>
        /// 移動したことをサーバに伝える
        /// </summary>
        Task MovePositionAsync(Vector3 position);
    }

    /// <summary>
    /// Server -> ClientのAPI
    /// </summary>
    public interface ISampleHubReceiver
    {
        /// <summary>
        /// 誰かがゲームに接続したことをクライアントに伝える
        /// </summary>
        void OnJoin(string userName);

        /// <summary>
        /// 誰かがゲームから切断したことをクライアントに伝える
        /// </summary>
        void OnLeave(string userName);

        /// <summary>
        /// 誰かが発言した事をクライアントに伝える
        /// </summary>
        void OnSendMessage(string userName, string message);

        /// <summary>
        /// 誰かが移動した事をクライアントに伝える
        /// </summary>
        void OnMovePosition(Player player);
    }
}