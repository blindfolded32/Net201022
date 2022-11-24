using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomView : MonoBehaviour
{
    [SerializeField] private GameObject _createRoomPanel;
    [SerializeField] private Button _createRoomButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private TMP_InputField _roomName;
    [SerializeField] private TMP_InputField _friendsList;
    [SerializeField] private TMP_InputField _password;
    [SerializeField] private Toggle _byFriendsToggle;
    [SerializeField] private Toggle _byPasswordToggle;

    public string RoomName { get; private set; }
    public string Password { get; private set; }
    public string FriendsList { get; private set; }
    public Button CreateRoomButton => _createRoomButton;
    public GameObject CreateRoomPanel => _createRoomPanel;
    public Toggle ByFriendsToggle => _byFriendsToggle;
    public Toggle ByPasswordToggle => _byPasswordToggle;

    private void Start()
    {
        _byFriendsToggle.isOn = false;
        _byPasswordToggle.isOn = false;
        _friendsList.interactable = false;
        _password.interactable = false;
        _createRoomButton.interactable = false;

        _roomName.onValueChanged.AddListener(UpdateRoomName);
        _friendsList.onValueChanged.AddListener(UpdateFriendsList);
        _password.onValueChanged.AddListener(UpdatePassword);
        _byPasswordToggle.onValueChanged.AddListener(UpdateByPasswordStatus);
        _byFriendsToggle.onValueChanged.AddListener(UpdateByFriendStatus);
        _backButton.onClick.AddListener(CloseCreationRoomPanel);
    }

    private void CloseCreationRoomPanel()
    {
        ClearAllData();
        _createRoomPanel.gameObject.SetActive(false);
    }

    private void ClearAllData()
    {
        RoomName = "";
        FriendsList = "";
        Password = "";
        _byFriendsToggle.isOn = false;
        _byPasswordToggle.isOn = false;
        _roomName.text = "";
        _password.text = "";
        _friendsList.text = "";
    }

    private void UpdateByFriendStatus(bool isPrivateByFriends)
    {
        _friendsList.interactable = isPrivateByFriends;
        if(isPrivateByFriends && _byPasswordToggle.isOn)
        {
            _byPasswordToggle.isOn = false;
        }
    }

    private void UpdateByPasswordStatus(bool isPrivateByPassword)
    {
        _password.interactable = isPrivateByPassword;
        if(isPrivateByPassword && _byFriendsToggle.isOn)
        {
            _byFriendsToggle.isOn = false;
        }
    }

    private void UpdatePassword(string password)
    {
        Password = password;
    }

    private void UpdateFriendsList(string friendList)
    {
        FriendsList = friendList;
    }

    private void UpdateRoomName(string roomName)
    {
        RoomName = roomName;
        if(roomName == "")
        {
            _createRoomButton.interactable = false;
        } else
        {
            _createRoomButton.interactable = true;
        }
    }

    private void OnDestroy()
    {
        _roomName.onValueChanged.RemoveAllListeners();
        _friendsList.onValueChanged.RemoveAllListeners();
        _password.onValueChanged.RemoveAllListeners();
        _byPasswordToggle.onValueChanged.RemoveAllListeners();
        _byFriendsToggle.onValueChanged.RemoveAllListeners();
        _backButton.onClick.RemoveAllListeners();
    }
}
