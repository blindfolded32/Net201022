using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionPanel : MonoBehaviour
{
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _connectButton;
    [SerializeField] private Button _disconnectButton;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private Image _statusImage;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Sprite _connectSprite;
    [SerializeField] private Sprite _disconnectSprite;
    [SerializeField] private TextMeshProUGUI _textHolder;

    private bool _isWarning;

    public Button LoginButton => _loginButton;
    public Button ConnectButton => _connectButton;
    public Button DisconnectButton => _disconnectButton;
    public bool IsWarning => _isWarning;

    public string GetLogin()
    {
        return _inputField.text;
    }

    public void LoginWarning()
    {
        _inputField.image.color = new Color(1, 0, 0, 0.5f);
        _textHolder.text = "Input login";
        _isWarning = true;
    }

    public void ResetWarning()
    {
        _inputField.image.color = Color.white;
        _isWarning = false;
    }

    public void SetOfflineConnectionStatus()
    {
        _statusImage.sprite = _disconnectSprite;
        _statusText.text = "<color=red>Offline</color>";
        _connectButton.gameObject.SetActive(true);
        _disconnectButton.gameObject.SetActive(false);
    }
    public void SetOnlineConnectionStatus()
    {
        _statusImage.sprite = _connectSprite;
        _statusText.text = "<color=green>Online</color>";
        _connectButton.gameObject.SetActive(false);
        _disconnectButton.gameObject.SetActive(true);
    }
}
