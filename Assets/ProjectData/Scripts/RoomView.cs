using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectData.Scripts
{
    public class RoomView : MonoBehaviour
    {
        [SerializeField] private Button _closeRoomButton;
        [SerializeField] private TMP_Text _closeButtonText;
        [SerializeField] private Button _hideRoomButton;
        [SerializeField] private TMP_Text _hideButtonText;
        [SerializeField] private Toggle _readyToggle;
        [SerializeField] private Button _startGameButton;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private List<PlayerSlotView> _playerInRoomViews;
        [SerializeField] private List<PlayerCharacterView> _characterViews;

        private bool _isVisible = true;
        private bool _isOpen = true;
        private bool _isAlreadyClicked = false;
        private string _roomOwner;
        private string _playerName;
        private PlayerCharacterView _selectedCharacter;
        private Dictionary<string, PlayerSlotView> _slotsByPlayers = new Dictionary<string, PlayerSlotView>();


        public Button CloseRoomButton => _closeRoomButton;
        public Button HideRoomButton => _hideRoomButton;
        public Button StartGameButton => _startGameButton;
        public Toggle ReadyToggle => _readyToggle;
        public PlayerCharacterView SelectedCharacter => _selectedCharacter;

        private void Start()
        {
            for (int i = 0; i < _characterViews.Count; i++)
            {
                _characterViews[i].OnClickCharacterView += SelectPickedCharacter;
            }
        }

        public void ShowRoom()
        {
            _nameText.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
            _roomOwner = PhotonNetwork.CurrentRoom.CustomProperties[ConstantsForPhoton.OWNER].ToString();
            _playerName = PhotonNetwork.LocalPlayer.NickName;

            if (_roomOwner != _playerName)
            {
                _closeRoomButton.gameObject.SetActive(false);
                _hideRoomButton.gameObject.SetActive(false);
                _startGameButton.gameObject.SetActive(false);
            } else
            {
                _closeRoomButton.onClick.AddListener(ChangeOpenRoomStatus);
                _hideRoomButton.onClick.AddListener(ChangeVisibleRoomStatus);
                _startGameButton.onClick.AddListener(StartGame);
                _startGameButton.interactable = false;
            }

            _readyToggle.onValueChanged.AddListener(ChangeLocalReadyStatus);
            _readyToggle.interactable = false;
        }

        public void LoadCharacters(List<CharacterResult> characters)
        {
            for(int i = 0; i < _characterViews.Count; i++)
            {
                if(i>=characters.Count) break;
                _characterViews[i].Init(characters[i]);
            }
        }

        private void SelectPickedCharacter(PlayerCharacterView character)
        {
            _selectedCharacter = character;

            for (int i = 0; i < _characterViews.Count; i++)
            {
                if (_characterViews[i] == character)
                {
                    if (!_characterViews[i].IsSelected)
                    {
                        _characterViews[i].SelectView();
                        SetReadyToggleInteractableState(true);
                    }
                    else
                    {
                        _characterViews[i].DeselectView();
                        _selectedCharacter = null;
                        SetReadyToggleInteractableState(false);
                    }
                }
                else
                {
                    if (_characterViews[i].IsSelected)
                    {
                        _characterViews[i].DeselectView();
                    }
                }
            }
        }

        private void ChangeLocalReadyStatus(bool value)
        {
            _slotsByPlayers[_playerName].ChangeReadyStatus(value);

            SendReadyStatusInfo(_playerName, value);
        }

        private void SendReadyStatusInfo(string name, bool value)
        {
            var customParameters = PhotonNetwork.CurrentRoom.CustomProperties;

            customParameters[ConstantsForPhoton.READY_STATUS] = value;
            customParameters[ConstantsForPhoton.READY_PLAYER] = _playerName;
            PhotonNetwork.CurrentRoom.SetCustomProperties(customParameters);

            var playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;

            playerProperties[ConstantsForPhoton.CHARACTER_ID] = _selectedCharacter is null ? "" : _selectedCharacter.CharacterResult.CharacterId;
            playerProperties[ConstantsForPhoton.CHARACTER_TYPE] = _selectedCharacter is null ? "" : _selectedCharacter.CharacterResult.CharacterType;
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        }

        public void ChangeGlobalReadyStatus((string, bool) readyCartage)
        {
            if (readyCartage.Item1 != _playerName)
            {
                _slotsByPlayers[readyCartage.Item1].ChangeReadyStatus(readyCartage.Item2);
            }

            CheckOverallReadiness();
        }

        private void CheckOverallReadiness()
        {
            if (_roomOwner == _playerName)
            {
                if (IsAllReady())
                {
                    _startGameButton.interactable = true;
                }
                else
                {
                    _startGameButton.interactable = false;
                }
            }
        }

        private bool IsAllReady()
        {
            for (int i = 0; i < _playerInRoomViews.Count; i++)
            {
                if(_playerInRoomViews[i].Player != null && !_playerInRoomViews[i].IsReady)
                {
                    return false;
                }
            }

            return true;
        }

        private void StartGame()
        {
            if(_isAlreadyClicked) return;
            PhotonNetwork.LoadLevel(2);
            _isAlreadyClicked = true;
            _startGameButton.interactable = false;
            ChangeOpenRoomStatus();
        }

        private void ChangeVisibleRoomStatus()
        {
            _isVisible = _isVisible ? false : true;
            PhotonNetwork.CurrentRoom.IsVisible = _isVisible;
            _hideButtonText.text = _isVisible ? "Hide" : "Show";
        }

        private void ChangeOpenRoomStatus()
        {
            _isOpen = _isOpen ? false : true;
            PhotonNetwork.CurrentRoom.IsOpen = _isOpen;
            _closeButtonText.text = _isOpen ? "Close" : "Open";
        }

        public void OnNewPlayerEnteredRoom(Player newPlayer)
        {
            FindAndTakeFreeSlot(newPlayer);
            SendReadyStatusInfo(_playerName, _slotsByPlayers[_playerName].IsReady);

            if (_roomOwner == _playerName)
            {
                CheckOnMaxPlayers();
                CheckOverallReadiness();
            }
        }

        public void OnJoinRoom()
        {     
            foreach(var player in PhotonNetwork.CurrentRoom.Players)
            {
                FindAndTakeFreeSlot(player.Value);
            }
        }

        private void CheckOnMaxPlayers()
        {
            if (_isOpen && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                _isOpen = false;
                PhotonNetwork.CurrentRoom.IsOpen = _isOpen;
                _closeRoomButton.interactable = _isOpen;
            }
            else
            {
                _closeRoomButton.interactable = true;
            }
        }

        private void FindAndTakeFreeSlot(Player newPlayer)
        {
            for (int i = 0; i < _playerInRoomViews.Count; i++)
            {
                if (!_playerInRoomViews[i].IsBusy)
                {
                    _playerInRoomViews[i].TakeSlot(newPlayer);
                    _slotsByPlayers.Add(newPlayer.NickName, _playerInRoomViews[i]);
                    break;
                }
            }
        }

        public void OnPlayerLeftRoom(Player player)
        {
            for(int i = 0; i < _playerInRoomViews.Count; i++)
            {
                if (_playerInRoomViews[i].Player != null && (_playerInRoomViews[i].Player.NickName == player.NickName))
                {
                    _playerInRoomViews[i].ClearSlot();
                }
            }

            if (_roomOwner == _playerName)
            {
                CheckOnMaxPlayers();
                CheckOverallReadiness();
            }
        }

        private void SetReadyToggleInteractableState(bool isInteractable)
        {
            _readyToggle.interactable = isInteractable;
            if(!isInteractable && _readyToggle.isOn)
            {
                _readyToggle.isOn = false;
            }
        }

        private void OnDestroy()
        {
            _closeRoomButton.onClick.RemoveAllListeners();
            _hideRoomButton.onClick.RemoveAllListeners();
            _startGameButton.onClick.RemoveAllListeners();
            _readyToggle.onValueChanged.RemoveAllListeners();

            for (int i = 0; i < _characterViews.Count; i++)
            {
                _characterViews[i].OnClickCharacterView -= SelectPickedCharacter;
            }
        }
    }
}
