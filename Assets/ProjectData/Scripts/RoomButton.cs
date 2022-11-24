using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour, IPointerClickHandler
{
    public event Action<RoomButton> OnClickRoomMiniView;
    public bool IsSelected { get; private set; }
    public RoomInfo RoomInfo { get; private set; }
    public bool IsCloseOrNotVisible { get; private set; }
    public RoomTypes RoomType { get; private set; }
    public string[] FriendsList { get; private set; }

    [SerializeField] private TMP_Text _roomName;
    [SerializeField] private TMP_Text _roomOwner;
    [SerializeField] private Image _backGroundImage;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _passwordColor;
    [SerializeField] private Color _friendsColor;
 
    private Color _idleColor;
    private Color _idleTextColor;
    private string _ownerName;
    private string _password;

    public void Init(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;

        if(RoomInfo.CustomProperties.ContainsKey(LobbyManager.OWNER))
        {
            _ownerName = RoomInfo.CustomProperties[LobbyManager.OWNER].ToString();
            _roomOwner.text = $"Owner: {_ownerName}";
        }

        if(RoomInfo.CustomProperties.ContainsKey(LobbyManager.PASSWORD))
        {
            RoomType = RoomTypes.PrivatByPassword;
            _password = RoomInfo.CustomProperties[LobbyManager.PASSWORD].ToString();
            _idleColor = _passwordColor;
        } 
        else if (RoomInfo.CustomProperties.ContainsKey(LobbyManager.FRIENDS))
        {
            RoomType = RoomTypes.PrivatByNickname;
            FriendsList = (string[])RoomInfo.CustomProperties[LobbyManager.FRIENDS];
            _idleColor = _friendsColor;
        }
        else
        {
            RoomType = RoomTypes.Normal;
            _idleColor = _normalColor;
        }

        _backGroundImage.color = _idleColor;
        _roomName.text = roomInfo.Name;
        _idleTextColor = _roomName.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsCloseOrNotVisible) return;

        OnClickRoomMiniView?.Invoke(this);
        Debug.Log("OnPointerClick");
    }

    public void SelectView()
    {
        if (IsCloseOrNotVisible) return;

        IsSelected = true;
        _backGroundImage.color = _selectedColor;
    }

    public void DeselectView()
    {
        if (IsCloseOrNotVisible) return;

        IsSelected = false;        
        _backGroundImage.color = _idleColor;
    }

    public void ClearRoomMiniView()
    {
        RoomInfo = null;
        _roomName.text = "";
    }

    public void SetUnavailableStatus()
    {
        IsCloseOrNotVisible = true;
        _backGroundImage.color = new Color(_idleColor.r, _idleColor.g, _idleColor.b, _idleColor.a * 0.2f);
        _roomName.color = new Color(_idleTextColor.r, _idleTextColor.g, _idleTextColor.b, _idleTextColor.a * 0.2f);
        _roomOwner.color = new Color(_idleTextColor.r, _idleTextColor.g, _idleTextColor.b, _idleTextColor.a * 0.2f);
    }

    public void TakeOffUnavailableStatus()
    {
        IsCloseOrNotVisible = false;
        _backGroundImage.color = _idleColor;
        _roomName.color = _idleTextColor;
        _roomOwner.color = _idleTextColor;
    }

    public bool CheckPassword(string password)
    {
        return _password.Equals(password);
    }

    public bool CheckFriendsList(string nickName)
    {
        for(int i = 0; i < FriendsList.Length; i++)
        {
            if (FriendsList[i].Equals(nickName)) return true;
        }

        return false;
    }
}
