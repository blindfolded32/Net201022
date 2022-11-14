using System;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectData.Scripts
{
    public class GameStartWindow : MonoBehaviour
    {
        [SerializeField] private Button _singInButton;
        [SerializeField] private Button _createAccountButton;
        [SerializeField] private Canvas _gameStartCanvas;
        [SerializeField] private Canvas _signInCanvas;
        [SerializeField] private Canvas _createAccountCanvas;

        private void Start()
        {
            _singInButton.onClick.AddListener(OpenSignInWindow);
            _createAccountButton.onClick.AddListener(OpenCreateAccountInWindow);
        }

        private void OpenCreateAccountInWindow()
        {
            _gameStartCanvas.enabled = false;
            _createAccountCanvas.enabled = true;
            _signInCanvas.enabled = false;
        }

        private void OpenSignInWindow()
        {
            _gameStartCanvas.enabled = false;
            _signInCanvas.enabled = true;
            _createAccountCanvas.enabled = false;
        }

        private void OnDestroy()
        {
            _singInButton.onClick.RemoveAllListeners();
            _createAccountButton.onClick.RemoveAllListeners();
        }
    }
     
    
}