using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using System;
using System.Collections.Generic;
using ProjectData.Scripts;

public class PlayersCreatorController
{
    private RoomView _roomView;

    public PlayersCreatorController(RoomView roomView) 
    {
        _roomView = roomView;
    }


    public void GetPlayerCharacters()
    {
        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(), OnGetPlayersSuccess, OnError);
    }

    private void OnGetPlayersSuccess(ListUsersCharactersResult result)
    {
        if (result.Characters.Count == 0)
        {
            CreateNewCharacters();
        }
        else
        {
            _roomView.LoadCharacters(result.Characters);
        }
    }

    private void CreateNewCharacters()
    {
        PlayFabClientAPI.GetStoreItems(new GetStoreItemsRequest
        {
            CatalogVersion = ConstantsForPlayFab.CATALOG_VERSION,
            StoreId = ConstantsForPlayFab.CHARACTER_STORE
        }, result =>
        {
            foreach (var item in result.Store)
            {
                GetPlayerToken(item);
            }
        }, OnError);

    }

    private void GetPlayerToken(StoreItem storeItem)
    {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
        {
            ItemId = storeItem.ItemId,
            Price = (int)storeItem.VirtualCurrencyPrices[ConstantsForPlayFab.GOLD],
            VirtualCurrency = ConstantsForPlayFab.GOLD
        }, result => CreatePlayerWithToken(result.Items[0].ItemId), OnError);
    }

    private void CreatePlayerWithToken(string itemID)
    {
        PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest
        {
            CharacterName = itemID switch
            {
                ConstantsForPlayFab.PLAYERPREFABID => "PlayerPref",
                ConstantsForPlayFab.PLAYERWSHIELDID => "PlayerPrefwShield"
            },
            ItemId = itemID
        }, result => SetNewPlayerStatistics(result), OnError);
    }

    private void SetNewPlayerStatistics(GrantCharacterToUserResult result)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdateCharacterStatisticsRequest
        {
            CharacterId = result.CharacterId,
            CharacterStatistics = new Dictionary<string, int>
            {
                [ConstantsForPlayFab.PLAYERHEALTH] = 100,
                [ConstantsForPlayFab.PLAYERDAMAGE] = result.CharacterType switch
                {
                    ConstantsForPlayFab.PLAYERPREFABID => 1,
                    ConstantsForPlayFab.PLAYERWSHIELDID => 1
                }
            }
        }, result => Debug.Log($"Initial stats set"), OnError); ;
    }

    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.Log(errorMessage);
    }
}