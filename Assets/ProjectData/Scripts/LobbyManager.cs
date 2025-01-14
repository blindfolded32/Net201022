using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;
using ProjectData.Scripts;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private LobbyView _lobbyView;
    [SerializeField] private CreateRoomView _createRoomView;
    [SerializeField] private RoomView _roomView;

    private PlayersCreatorController _charactersCreatorController;


    private string _playerName;
    private string _playerID;
    private string[] _friendsList;

    private void Start()
    {
        _lobbyView.SetOfflineConnectionStatus();

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountSuccess, OnError);
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess, OnError);

        _lobbyView.ConnectButton.onClick.AddListener(Connect);
        _lobbyView.DisconnectButton.onClick.AddListener(Disconnect);
        _lobbyView.CreateRoomButton.onClick.AddListener(ShowRoomCreationPanel);
        _lobbyView.JoinToRoomButton.onClick.AddListener(JoinToRoom);
        _lobbyView.AcceptPasswordButton.onClick.AddListener(CheckPassword);
        _lobbyView.CancelPasswordButton.onClick.AddListener(ClosePasswordPanel);
        _createRoomView.CreateRoomButton.onClick.AddListener(CreateRoom);

        _charactersCreatorController = new PlayersCreatorController(_roomView);
        _charactersCreatorController.GetPlayerCharacters();
    }

    private void ClosePasswordPanel()
    {
        _lobbyView.PasswordPanel.SetActive(false);
    }

    private void CheckPassword()
    {
        if(_lobbyView.SelectedRoom.CheckPassword(_lobbyView.PasswordField.text))
        {
            PhotonNetwork.JoinRoom(_lobbyView.SelectedRoom.RoomInfo.Name);
            _lobbyView.PasswordPanel.SetActive(false);
        } else
        {
            _lobbyView.PasswordField.text = "WRONG!!!!";
        }
    }

    private void JoinToRoom()
    {
        if(_lobbyView.SelectedRoom.RoomType == RoomTypes.PrivatByPassword)
        {
            _lobbyView.PasswordPanel.SetActive(true);
        }
        else
        {
            PhotonNetwork.JoinRoom(_lobbyView.SelectedRoom.RoomInfo.Name);
        }
    }

    private void ShowRoomCreationPanel()
    {
        _createRoomView.CreateRoomPanel.gameObject.SetActive(true);
    }

    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.Log(errorMessage);
    }

    private void OnGetAccountSuccess(GetAccountInfoResult result)
    {
        var accountInfo = result.AccountInfo;
        _playerID = accountInfo.PlayFabId;

        _playerName = accountInfo.Username ?? $"user{_playerID}";

        _lobbyView.AccauntInfo.text = $"Hello {_playerName} \n" +
                                      $"Your ID is {_playerID} \n";
    }

    private void OnGetCatalogSuccess(GetCatalogItemsResult result)
    {
        WriteCatalog(result.Catalog);
        Debug.Log("Catalog is loaded");
    }

    private void WriteCatalog(List<CatalogItem> catalog)
    {
        _lobbyView.Catalog.text = "Items catalog:";

        foreach (var item in catalog)
        {
            _lobbyView.Catalog.text += $"\n{item.DisplayName}";                                 
        }
    }

    private void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.AuthValues = new AuthenticationValues(_playerID);
        PhotonNetwork.NickName = _playerName;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = PhotonNetwork.AppVersion;
        _lobbyView.SetPlayerName(_playerName);
    }

    private void CreateRoom()
    {
        var roomName = _createRoomView.RoomName;
        RoomOptions roomOptions = new RoomOptions 
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true,
            PublishUserId = true,
            PlayerTtl = 10000
        };
        
        if(_createRoomView.ByFriendsToggle.isOn)
        {
            _friendsList = _createRoomView.FriendsList.Split(",");

            var customRoomProperties = new Hashtable 
            { { ConstantsForPhoton.MAP_PROP_KEY, "Map_3" }, 
                { ConstantsForPhoton.OWNER, _playerName },
                { ConstantsForPhoton.READY_PLAYER, "" },
                { ConstantsForPhoton.READY_STATUS, false }, 
                { ConstantsForPhoton.FRIENDS, _friendsList } 
            };

            var customRoomPropertiesForLobby = new[] { ConstantsForPhoton.MAP_PROP_KEY, ConstantsForPhoton.OWNER,
                ConstantsForPhoton.READY_PLAYER, ConstantsForPhoton.READY_STATUS, ConstantsForPhoton.FRIENDS };

            roomOptions.CustomRoomProperties = customRoomProperties;
            roomOptions.CustomRoomPropertiesForLobby = customRoomPropertiesForLobby;

        } 
        else if (_createRoomView.ByPasswordToggle.isOn)
        {
            var password = _createRoomView.Password;
            var customRoomProperties = new Hashtable { { ConstantsForPhoton.MAP_PROP_KEY, "Map_3" }, { ConstantsForPhoton.OWNER, _playerName }, { ConstantsForPhoton.READY_PLAYER, "" },
                { ConstantsForPhoton.READY_STATUS, false }, { ConstantsForPhoton.PASSWORD, password } };
            var customRoomPropertiesForLobby = new[] { ConstantsForPhoton.MAP_PROP_KEY, ConstantsForPhoton.OWNER,
                ConstantsForPhoton.READY_PLAYER, ConstantsForPhoton.READY_STATUS, ConstantsForPhoton.PASSWORD };

            roomOptions.CustomRoomProperties = customRoomProperties;
            roomOptions.CustomRoomPropertiesForLobby = customRoomPropertiesForLobby;
        }
        else
        {
            var customRoomProperties = new Hashtable { { ConstantsForPhoton.MAP_PROP_KEY, "Map_3" }, { ConstantsForPhoton.READY_PLAYER, "" },
                { ConstantsForPhoton.READY_STATUS, false }, { ConstantsForPhoton.OWNER, _playerName }};
            var customRoomPropertiesForLobby = new[] { ConstantsForPhoton.MAP_PROP_KEY, ConstantsForPhoton.OWNER, ConstantsForPhoton.READY_PLAYER, ConstantsForPhoton.READY_STATUS };

            roomOptions.CustomRoomProperties = customRoomProperties;
            roomOptions.CustomRoomPropertiesForLobby = customRoomPropertiesForLobby;
        }

        PhotonNetwork.CreateRoom(roomName, roomOptions);

        Debug.Log("CreateRoom");
        _createRoomView.CreateRoomPanel.SetActive(false);
    }

    private void Disconnect()
    {
        PhotonNetwork.Disconnect();
        _lobbyView.ClearRoomList();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinLobby();
        _lobbyView.SetOnlineConnectionStatus();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        Debug.Log("OnJoinedLobby");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        OpenRoomView();
        Debug.Log("OnCreatedRoom");
    }

    private void OpenRoomView()
    {
        _lobbyView.LobbyPanel.SetActive(false);
        _roomView.gameObject.SetActive(true);
        _roomView.ShowRoom();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log($"OnJoinedRoom {PhotonNetwork.CurrentRoom.Name}");
        OpenRoomView();
        _roomView.OnJoinRoom();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        _lobbyView.UpdateRoomListView(roomList);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Disconnected");
        _lobbyView.SetOfflineConnectionStatus();
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        Debug.Log("Left Lobby");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        _roomView.OnNewPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        _roomView.OnPlayerLeftRoom(otherPlayer);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        (string, bool) readyCartage;

        if(propertiesThatChanged.ContainsKey(ConstantsForPhoton.READY_STATUS))
        {
            readyCartage.Item1 = (string)propertiesThatChanged[ConstantsForPhoton.READY_PLAYER];
            readyCartage.Item2 = (bool)propertiesThatChanged[ConstantsForPhoton.READY_STATUS];

            _roomView.ChangeGlobalReadyStatus(readyCartage);
        }
    }

    private void OnDestroy()
    {
        _lobbyView.ConnectButton.onClick.RemoveAllListeners();
        _lobbyView.DisconnectButton.onClick.RemoveAllListeners();
        _lobbyView.CreateRoomButton.onClick.RemoveAllListeners();
        _lobbyView.JoinToRoomButton.onClick.RemoveAllListeners();
        _lobbyView.AcceptPasswordButton.onClick.RemoveAllListeners();
        _lobbyView.CancelPasswordButton.onClick.RemoveAllListeners();
        _createRoomView.CreateRoomButton.onClick.RemoveAllListeners();
    }
}
