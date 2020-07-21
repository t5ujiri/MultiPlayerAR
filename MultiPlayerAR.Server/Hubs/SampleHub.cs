using MagicOnion.Server.Hubs;
using System.Threading.Tasks;
using net.caffeineinject.multiplayerar.servershared.hubs;
using net.caffeineinject.multiplayerar.servershared.messagepackobjects;
using UnityEngine;

namespace net.caffeineinject.multiplayerar.server.hubs
{
    public class SampleHub : StreamingHubBase<ISampleHub, ISampleHubReceiver>, ISampleHub
    {
        IGroup _room;
        Player _me;

        public async Task JoinAsync(Player player)
        {
            //ルームは全員固定
            const string roomName = "SampleRoom";
            //ルームに参加&ルームを保持
            this._room = await this.Group.AddAsync(roomName);
            //自分の情報も保持
            _me = player;
            //参加したことをルームに参加している全メンバーに通知
            this.Broadcast(_room).OnJoin(_me.Name);
        }

        public async Task LeaveAsync()
        {
            //ルーム内のメンバーから自分を削除
            await _room.RemoveAsync(this.Context);
            //退室したことを全メンバーに通知
            this.Broadcast(_room).OnLeave(_me.Name);
        }

        public async Task SendMessageAsync(string message)
        {
            //発言した内容を全メンバーに通知
            this.Broadcast(_room).OnSendMessage(_me.Name, message);
        }

        public async Task MovePositionAsync(Vector3 position)
        {
            // サーバー上の情報を更新
            _me.Position = position;

            //更新したプレイヤーの情報を全メンバーに通知
            this.Broadcast(_room).OnMovePosition(_me);
        }

        protected override ValueTask OnDisconnected()
        {
            //nop
            return CompletedTask;
        }
    }
}