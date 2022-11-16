using System.Linq;
using System.Net;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectData.Scripts
{
    public class SingInWindow : AccountDataWindow
    {
        [SerializeField] private Button _signInButton;
        [SerializeField] private Canvas _canvas;
        
        private const string AUTHINTEFICATION_KEY = "AUTHKEY";
        private string _customID;
        private bool _isNeedCreation;
        protected override void SubcribeUIElements()
        {
            base.SubcribeUIElements();
            _signInButton.onClick.AddListener(SignIn);
        }

        protected override void OpenStartWindow()
        {
            _canvas.enabled = false;
            ResetProfile();
            base.OpenStartWindow();
        }

        private void SignIn()
        {
        /*    PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
            {
                Username =  _login,
                Password = _password
            },Success, Fail);*/
        
        _isNeedCreation = _customID == PlayerPrefs.GetString(AUTHINTEFICATION_KEY) ? false : true;
        _customID = _login;

        if (_isNeedCreation)
        {
            PlayerPrefs.SetString(AUTHINTEFICATION_KEY, _customID);
        }
        var request = new LoginWithCustomIDRequest
        {
            CustomId = _customID,
            CreateAccount = _isNeedCreation
        };

        PlayFabClientAPI.LoginWithCustomID(request, Success, Fail, GetLocalIPv4());
        StartLoginCoroutine();
        _signInButton.interactable = false;
        }
        
        private string GetLocalIPv4()
        {
            return Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.First(
                    f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();
        }
        private void ResetProfile()
        {
            PlayerPrefs.DeleteKey(AUTHINTEFICATION_KEY);
            _loginField.text = "";
            _passwordField.text = "";
        }

        protected override void Success(LoginResult result)
        {
            base.Success(result);
            _canvas.enabled = false;
        }

    }
}