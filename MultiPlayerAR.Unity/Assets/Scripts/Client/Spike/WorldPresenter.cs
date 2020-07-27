using System;
using System.Collections.Generic;
using System.Text;
using Client.Application.Service;
using Client.Presentation.View;
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
        [SerializeField] private Camera mainCamera = default;
        [SerializeField] private Transform anchorObject = default;

        [SerializeField] private TMP_InputField roomName = default;
        [SerializeField] private TMP_InputField userName = default;
        [SerializeField] private Button joinButton = default;
        [SerializeField] private Button leaveButton = default;
        [SerializeField] private TMP_InputField messageInputField = default;
        [SerializeField] private Button sayButton = default;
        [SerializeField] private TextMeshProUGUI log = default;
        [SerializeField] private PlayerView playerViewPrefab = default;


        private ARWorldClient _arWorldClient;
        private readonly string _playerId = Guid.NewGuid().ToString();
        private ARWorld _arWorld;
        private bool _isConnecting = false;
        private Vector3 _lastPosition = new Vector3();
        private Dictionary<string, PlayerView> _playerViews = new Dictionary<string, PlayerView>();

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
                                    InstantiatePlayerView(playerJoined);
                                    AppendLog($"Player {playerJoined.PlayerName} Joined.");
                                    break;
                                case PlayerLeft playerLeft:
                                    _arWorld?.Mutate(e);
                                    DestroyPlayerView(playerLeft);
                                    AppendLog($"Player {playerLeft.PlayerName} Left.");
                                    break;
                                case PlayerSpoke playerSpoke:
                                    _arWorld?.Mutate(e);
                                    AppendLog($"{playerSpoke.PlayerName} : {playerSpoke.Message}");
                                    break;
                                case PlayerMoved playerMoved:
                                    _arWorld?.Mutate(e);
                                    UpdatePlayerView(playerMoved);
                                    break;
                                case ARWorldEventStream arWorldEventStream:
                                    _arWorld = new ARWorld(arWorldEventStream.RoomName, arWorldEventStream.EventStream,
                                        new DomainEventPublisher());
                                    RestoreView(_arWorld);
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
                    _isConnecting = true;
                });

            leaveButton.OnClickAsObservable().TakeUntilDestroy(this)
                .Subscribe(async _ =>
                {
                    if (_arWorldClient == null) return;

                    await _arWorldClient.ExecuteAsync(new LeaveCommand()
                    {
                        PlayerId = _playerId
                    });
                    _isConnecting = false;
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

            Observable.EveryUpdate().Where(_ => _isConnecting).Subscribe(async _ =>
            {
                var pos = mainCamera.transform.position - anchorObject.transform.position;
                var rot = Quaternion.Inverse(anchorObject.transform.rotation) * mainCamera.transform.rotation;
                rot.ToAngleAxis(out var angle, out var axis);
                if (Vector3.Distance(_lastPosition, pos) > 0.01f | angle > 5)
                {
                    _lastPosition = pos;
                    await _arWorldClient.ExecuteAsync(new PlayerMoveCommand()
                    {
                        PlayerId = _playerId,
                        Position = pos,
                        Rotation = rot
                    });
                }
            });
        }

        private void RestoreView(ARWorld arWorld)
        {
            foreach (var playerView in FindObjectsOfType<PlayerView>())
            {
                Destroy(playerView.gameObject);
            }

            _playerViews.Clear();

            foreach (var keyValuePair in arWorld.Players)
            {
                var player = keyValuePair.Value;
                InstantiatePlayerView(
                    new PlayerJoined(arWorld.RoomName, player.PlayerId, player.PlayerName,
                        player.Position, player.Rotation));
            }
        }

        private void InstantiatePlayerView(PlayerJoined playerJoined)
        {
            var view = Instantiate(playerViewPrefab);
            view.userNameText.text = playerJoined.PlayerName;
            _playerViews.Add(playerJoined.PlayerId, view);
        }

        private void DestroyPlayerView(PlayerLeft playerLeft)
        {
            if (!_playerViews.ContainsKey(playerLeft.PlayerId)) return;
            Destroy(_playerViews[playerLeft.PlayerId].gameObject);
            _playerViews.Remove(playerLeft.PlayerId);
        }

        private void UpdatePlayerView(PlayerMoved playerMoved)
        {
            _playerViews[playerMoved.PlayerId]?.transform.SetPositionAndRotation(
                anchorObject.transform.position + playerMoved.Position,
                playerMoved.Rotation * anchorObject.rotation
            );
        }

        private void AppendLog(string message)
        {
            var sb = new StringBuilder(log.text);
            log.text = sb.AppendLine(message).ToString();
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();

            if (_arWorldClient != null)
            {
                
            }
            
            _arWorldClient?.DisposeAsync().Forget();
        }
    }
}