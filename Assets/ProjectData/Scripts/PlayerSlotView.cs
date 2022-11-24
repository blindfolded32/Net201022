using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlotView : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private TMP_Text _playerClassText;
    [SerializeField] private Image _readyImage;
    [SerializeField] private Sprite _readySprite;
    [SerializeField] private Sprite _notReadySprite;

    public Player Player { get; private set; }
    public bool IsBusy { get; private set; }
    public bool IsReady { get; private set; }

    public void TakeSlot(Player player)
    {
        Player = player;
        _playerNameText.text = "Player: " + player.NickName;
        IsBusy = true;
    }

    public void ClearSlot()
    {
        Player = null;
        _playerNameText.text = "Player: ";
        IsBusy = false;
        _readyImage.sprite = _notReadySprite;
    }

    public void ChangeReadyStatus(bool value)
    {
        IsReady = value;
        _readyImage.sprite = IsReady ? _readySprite : _notReadySprite;
    }
    
}
