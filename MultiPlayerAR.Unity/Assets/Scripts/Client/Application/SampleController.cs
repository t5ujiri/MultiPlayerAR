using Grpc.Core;
using MagicOnion.Client;
using net.caffeineinject.multiplayerar.servershared.hubs;
using net.caffeineinject.multiplayerar.servershared.messagepackobjects;
using net.caffeineinject.multiplayerar.servershared.services;
using UnityEngine;

public class SampleController : MonoBehaviour, ISampleHubReceiver
{
    private Channel _channel;
    private ISampleService _sampleService;
    private ISampleHub _sampleHub;

    private async void Start()
    {
        _channel = new Channel("localhost:12345", ChannelCredentials.Insecure);
        _sampleService = MagicOnionClient.Create<ISampleService>(_channel);
        _sampleHub = StreamingHubClient.Connect<ISampleHub, ISampleHubReceiver>(this._channel, this);

        SampleServiceTest(1, 2);

        SampleHubTest();
    }

    private async void OnDestroy()
    {
        await this._sampleHub.DisposeAsync();
        await this._channel.ShutdownAsync();
    }

    /// <summary>
    /// 普通のAPI通信のテスト用のメソッド
    /// </summary>
    private async void SampleServiceTest(int x, int y)
    {
        var sumResult = await this._sampleService.SumAsync(x, y);
        Debug.Log($"{nameof(sumResult)}: {sumResult}");

        var productResult = await this._sampleService.ProductAsync(2, 3);
        Debug.Log($"{nameof(productResult)}: {productResult}");
    }

    /// <summary>
    /// リアルタイム通信のテスト用のメソッド
    /// </summary>
    private async void SampleHubTest()
    {
        // 自分のプレイヤー情報を作ってみる
        var player = new Player
        {
            Name = "Minami",
            Position = new Vector3(0, 0, 0),
            Rotation = new Quaternion(0, 0, 0, 0)
        };

        // ゲームに接続する
        await this._sampleHub.JoinAsync(player);

        // チャットで発言してみる
        await this._sampleHub.SendMessageAsync("こんにちは！");

        // 位置情報を更新してみる
        player.Position = new Vector3(1, 0, 0);
        await this._sampleHub.MovePositionAsync(player.Position);

        // ゲームから切断してみる
        await this._sampleHub.LeaveAsync();
    }

    #region リアルタイム通信でサーバーから呼ばれるメソッド群

    public void OnJoin(string userName)
    {
        Debug.Log($"{userName}さんが入室しました");
    }

    public void OnLeave(string userName)
    {
        Debug.Log($"{userName}さんが退室しました");
    }

    public void OnSendMessage(string userName, string message)
    {
        Debug.Log($"{userName}: {message}");
    }

    public void OnMovePosition(Player player)
    {
        Debug.Log($"{player.Name}さんが移動しました: {{ x: {player.Position.x}, y: {player.Position.y}, z: {player.Position.z} }}");
    }

    #endregion
}