using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;


namespace ProjectData.Scripts
{
    public class PlayFabAccountManager
    {
        [SerializeField] private Text _titleLabel;
        private void Start()
        {
            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(),
                OnGetAccountSuccess, OnFailure);
        }
        private void OnGetAccountSuccess(GetAccountInfoResult result)
        {
            _titleLabel.text = $"Welcome back, Player ID {result.AccountInfo.PlayFabId}";
        }
        private void OnFailure(PlayFabError error)
        {
            var errorMessage = error.GenerateErrorReport();
            Debug.LogError($"Something went wrong: {errorMessage}");
        }

    }
}