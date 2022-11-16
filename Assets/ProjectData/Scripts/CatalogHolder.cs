using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

namespace ProjectData.Scripts
{
    public class CatalogHolder : MonoBehaviour
    {
        private readonly Dictionary<string, CatalogItem> _catalog = new Dictionary<string,
            CatalogItem>();
        [SerializeField]private TMP_Text _catalogHolder;
        
        
        public void Init()
        {
            PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess,
                OnFailure);
        }
        private void OnFailure(PlayFabError error)
        {
            var errorMessage = error.GenerateErrorReport();
            Debug.LogError($"Something went wrong: {errorMessage}");
        }
        private void OnGetCatalogSuccess(GetCatalogItemsResult result)
        {
            HandleCatalog(result.Catalog);
            Debug.Log($"Catalog was loaded successfully!");
        }
        private void HandleCatalog(List<CatalogItem> catalog)
        {
            foreach (var item in catalog)
            {
                _catalog.Add(item.ItemId, item);
                Debug.Log($"Catalog item {item.ItemId} was added successfully!");
            }
            ShowCatalog(catalog);
        }

        private void ShowCatalog(List<CatalogItem> catalog)
        {
            _catalogHolder.color = Color.white;
            
            _catalogHolder.text = "Items catalog:";

            foreach (var item in catalog)
            {
                _catalogHolder.text += $"\n{item.DisplayName}";                                 
            }
        }

    }
}