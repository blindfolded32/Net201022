using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectData.Scripts
{
    public class CreateAccountWindow : AccountDataWindow
    {
        [SerializeField] private InputField _emailField;
        [SerializeField] private Button _createAccountButton;
        [SerializeField] private Canvas _canvas;

        private string _email;
        
        private void UpdateEmail(string email)
        {
            _email = email;
        }

        private void CreateAccount()
        {
            PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
            {
                Username = _login,
                Email = _email,
                Password = _password
            }, result =>
            {
                Debug.Log($"Good: {_login} regstered"); 
            }, Fail);
            StartLoginCoroutine();
        }

        protected override void OpenStartWindow()
        {
            _canvas.enabled = false;
            base.OpenStartWindow();
        }

        protected override void SubcribeUIElements()
        {
            base.SubcribeUIElements();
            _emailField.onValueChanged.AddListener(UpdateEmail);
            _createAccountButton.onClick.AddListener(CreateAccount);
        }
        protected override void Success(LoginResult result)
        {
            base.Success(result);
            _canvas.enabled = false;
        }
    }
}