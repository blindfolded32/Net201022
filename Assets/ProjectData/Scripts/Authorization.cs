using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class Authorization : MonoBehaviourPunCallbacks
{
    [SerializeField] private string _playFabTitle;
    [SerializeField] private ConnectionPanel _connectionPanel;

    private string _customID;

    void Start()
    {
        _connectionPanel.SetOfflineConnectionStatus();
        _connectionPanel.ConnectButton.interactable = false;

        _connectionPanel.LoginButton.onClick.AddListener(Login);
        _connectionPanel.ConnectButton.onClick.AddListener(Connect);
        _connectionPanel.DisconnectButton.onClick.AddListener(Disconnect);
    }

    private void Login()
    {
        if (string.IsNullOrEmpty(_connectionPanel.GetLogin()))
        {
            _connectionPanel.LoginWarning();
            return;
        }
        else
        {
            if(_connectionPanel.IsWarning)
            {
                _connectionPanel.ResetWarning();
            }
            _customID = _connectionPanel.GetLogin();
        }

        if(string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            PlayFabSettings.staticSettings.TitleId = _playFabTitle;

        var request = new LoginWithCustomIDRequest
        {
            CustomId = _customID,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request,
            result =>
            {
                Debug.Log(result.PlayFabId);
                PhotonNetwork.AuthValues = new AuthenticationValues(result.PlayFabId);
                PhotonNetwork.NickName = result.PlayFabId;
                                
                _connectionPanel.ConnectButton.interactable = true;
                _connectionPanel.DisconnectButton.interactable = true;
            },
            error =>
            {
                Debug.LogError(error);
            });
    }

    private void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{Random.Range(0, 9999)}");
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = PhotonNetwork.AppVersion;
        }        
    }

    private void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected To Master");
        if (!PhotonNetwork.InRoom)
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{Random.Range(0, 9999)}");

        _connectionPanel.SetOnlineConnectionStatus();
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Created Room");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log($"Joined {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Disconnect");
        _connectionPanel.SetOfflineConnectionStatus();
    }

    private void OnDestroy()
    {
        _connectionPanel.LoginButton.onClick.RemoveAllListeners();
        _connectionPanel.ConnectButton.onClick.RemoveAllListeners();
        _connectionPanel.DisconnectButton.onClick.RemoveAllListeners();
    }
}
