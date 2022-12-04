using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace ProjectData.Scripts
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _playerwShieldPrefab;
        [SerializeField] private List<Transform> _spawnPoints;

        private void Start()
        {
            var playerSlotNumber = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            var playerCharacterType = PhotonNetwork.LocalPlayer.CustomProperties[ConstantsForPhoton.CHARACTER_TYPE].ToString();
            PlayerController character = default;

            if (playerCharacterType == ConstantsForPlayFab.PLAYERWSHIELDID)
            {
                var PlayerControllerGameObject = PhotonNetwork.Instantiate(_playerwShieldPrefab.name, _spawnPoints[playerSlotNumber].position, _spawnPoints[playerSlotNumber].rotation);
                character = PlayerControllerGameObject.GetComponent<PlayerController>();
            } else
            {
                var PlayerControllerGameObject = PhotonNetwork.Instantiate(_playerPrefab.name, _spawnPoints[playerSlotNumber].position, _spawnPoints[playerSlotNumber].rotation);
                character = PlayerControllerGameObject.GetComponent<PlayerController>();
            }

            PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest
            {
                CharacterId = PhotonNetwork.LocalPlayer.CustomProperties[ConstantsForPhoton.CHARACTER_ID].ToString()
            }, result => LoadCharacterParameters(result, character), error => Debug.Log(""));
        }

        private void LoadCharacterParameters(GetCharacterStatisticsResult result, PlayerController character)
        {
            var hp = result.CharacterStatistics[ConstantsForPlayFab.PLAYERHEALTH];
            var dmg = result.CharacterStatistics[ConstantsForPlayFab.PLAYERDAMAGE];
        
            character.SetCharacterParameters(hp,dmg);

            SendCharacterParameters(character.PhotonView.ViewID, hp, dmg);
        }

        private void SendCharacterParameters(int viewID, float hp, float dmg)
        {
            ReceiverGroup receiverGroup = ReceiverGroup.All;
            RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
            SendOptions sendOptions = new SendOptions { Reliability = true };

            object[] eventContent = new object[]
            {
                viewID,
                hp,
                dmg
            };

            PhotonNetwork.RaiseEvent((int)GameEvents.SyncParam, eventContent, options, sendOptions);
            Debug.Log("SendParametersEvent");
        }
    }
}
