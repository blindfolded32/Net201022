using System;
using System.Collections;
using System.Linq;
using System.Net;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectData.Scripts
{
    public class AccountDataWindow : MonoBehaviour
    {
        [SerializeField] protected InputField _loginField;
        [SerializeField] protected InputField _passwordField;
        [SerializeField] private Button _backButton;
        [SerializeField] private Canvas _startWindowCanvas;
        [SerializeField] private Text _statusText;
        [SerializeField] private Sprite _loadSprite;
        [SerializeField] private Image _statusImage;
        [SerializeField] private Canvas _catalogCanvas;
        private CatalogHolder _catalogHolder;

        protected string _login;
        protected string _password;
        
        public bool IsLogginInProgress;

        private void Start()
        {
            SubcribeUIElements();
            _catalogHolder = FindObjectOfType<CatalogHolder>();
        }
        private void UpdateLogin(string login)
        {
            _login = login;
        }
        private void UpdatePassword(string password)
        {
            _password = password;
        }
        private IEnumerator LoginProgressCoroutine()
        {
            while (IsLogginInProgress)
            {
                _statusImage.transform.Rotate(Vector3.forward * Time.deltaTime * 100);
                yield return new WaitForSeconds(3.0f);
                IsLogginInProgress = false;
            }
            _statusImage.transform.rotation = Quaternion.identity;
            _statusImage.enabled = false;
        }

        private void ShowCatalogWindow()
        {
            _catalogHolder.Init();
            _catalogCanvas.enabled = true;
        }
        protected  void StartLoginCoroutine()
        {
            _statusImage.enabled = true;
            _statusImage.sprite = _loadSprite;
            _statusText.text = "<color=white>Connecting</color>";
            IsLogginInProgress = true;
            StartCoroutine(LoginProgressCoroutine());
        }
        protected virtual void OpenStartWindow()
        {
            _startWindowCanvas.enabled = true;
        }
        protected virtual void SubcribeUIElements()
        {
            _loginField.onValueChanged.AddListener(UpdateLogin);
            _passwordField.onValueChanged.AddListener(UpdatePassword);
            _backButton.onClick.AddListener(OpenStartWindow);
        }
        protected void Fail(PlayFabError error)
        {
            Debug.LogError(error);
            _statusText.text = error.ToString();
        }

        protected virtual void Success(LoginResult result)
        {
            Debug.Log(result.PlayFabId);
            Debug.Log((string)result.CustomData);
            PhotonNetwork.AuthValues = new AuthenticationValues(result.PlayFabId);
            PhotonNetwork.NickName = result.PlayFabId;
            _statusText.color = Color.green;
            _statusText.text = $"Player: {result.PlayFabId} \nIP: {(string) result.CustomData}";
            ShowCatalogWindow();
        }
        
        
    }
}