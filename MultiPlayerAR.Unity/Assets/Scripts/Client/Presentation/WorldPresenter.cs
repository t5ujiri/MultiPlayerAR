using System;
using System.Text;
using Client.Application.Service;
using Cysharp.Threading.Tasks;
using net.caffeineinject.multiplayerar.domain;
using net.caffeineinject.multiplayerar.servershared.messagepackobjects;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Client.Presentation
{
    public class WorldPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_InputField roomName = default;
        [SerializeField] private TMP_InputField userName = default;
        [SerializeField] private Button joinButton = default;
        [SerializeField] private Button leaveButton = default;
        [SerializeField] private TMP_InputField messageInputField = default;
        [SerializeField] private Button sayButton = default;
        [SerializeField] private TextMeshProUGUI log = default;

        private ARWorldClient _arWorldClient;
        private readonly string _playerId = Guid.NewGuid().ToString();
        private ARWorld _arWorld;

        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private void Start()
        {
            joinButton.OnClickAsObservable().TakeUntilDestroy(this)
                .Subscribe(async _ =>
                {
                    _arWorldClient = new ARWorldClient();
                    
                    _arWorldClient.OnEventReceived.TakeUntilDestroy(this)
                        .Subscribe(e =>
                        {
                            switch (e)
                            {
                                case PlayerJoined playerJoined:
                                    _arWorld?.Mutate(e);
                                    AppendLog($"Player {playerJoined.PlayerName} Joined.");
                                    break;
                                case PlayerLeft playerLeft:
                                    AppendLog($"Player {playerLeft.PlayerName} Left.");
                                    _arWorld?.Mutate(e);
                                    break;
                                case PlayerSpoke playerSpoke:
                                    AppendLog($"{playerSpoke.PlayerName} : {playerSpoke.Message}");
                                    _arWorld?.Mutate(e);
                                    break;
                                case ARWorldEventStream arWorldEventStream:
                                    _arWorld = new ARWorld(arWorldEventStream.RoomName, arWorldEventStream.EventStream,
                                        new DomainEventPublisher());
                                    AppendLog(
                                        $"EventStreamLength: {arWorldEventStream.EventStream.Events.Count} / Messages : {_arWorld.Messages.Count}");
                                    foreach (var message in _arWorld.Messages)
                                    {
                                        AppendLog($"restored {message.PlayerName} : {message.Message}");
                                    }

                                    break;
                            }
                        }).AddTo(_compositeDisposable);
                    _arWorldClient.Connect();

                    await _arWorldClient.ExecuteAsync(new JoinCommand()
                    {
                        RoomName = roomName.text,
                        PlayerId = _playerId,
                        PlayerName = userName.text,
                        Position = new Vector3(),
                        Rotation = new Quaternion()
                    });
                });

            leaveButton.OnClickAsObservable().TakeUntilDestroy(this)
                .Subscribe(async _ =>
                {
                    if (_arWorldClient == null) return;

                    await _arWorldClient.ExecuteAsync(new LeaveCommand()
                    {
                        PlayerId = _playerId
                    });
                    await _arWorldClient.DisposeAsync();
                    _arWorldClient = null;
                    _compositeDisposable.Clear();
                    log.text = "";
                });

            sayButton.OnClickAsObservable().TakeUntilDestroy(this)
                .Subscribe(async _ =>
                {
                    if (_arWorldClient == null) return;

                    await _arWorldClient.ExecuteAsync(
                        new PlayerSayCommand()
                        {
                            PlayerId = _playerId,
                            Message = messageInputField.text
                        });
                });
        }

        private void AppendLog(string message)
        {
            var sb = new StringBuilder(log.text);
            log.text = sb.AppendLine(message).ToString();
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
            _arWorldClient?.DisposeAsync().Forget();
        }
    }
}