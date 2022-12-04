using PlayFab;
using PlayFab.ClientModels;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCharacterView : MonoBehaviour, IPointerClickHandler
{
    public event Action<PlayerCharacterView> OnClickCharacterView;
    public bool IsSelected { get; private set; }
    public CharacterResult CharacterResult => _characterResult;

    [SerializeField] private TMP_Text _classText;
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private TMP_Text _dmgText;
    [SerializeField] private Image _classImage;
    [SerializeField] private Image _backGroundImage;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _normalColor;
    [SerializeField] private Sprite _playerSprite;
    [SerializeField] private Sprite _playerwithshieldSprite;

    private Color _idleColor;
    private CharacterResult _characterResult;

    private void Start()
    {
        _idleColor = _backGroundImage.color;
    }
    
    public void Init(CharacterResult characterResult)
    {
        _characterResult = characterResult;
        _classText.text = characterResult.CharacterName;

        _classImage.sprite = characterResult.CharacterType switch
        {
            ConstantsForPlayFab.PLAYERPREFABID => _playerSprite,
            ConstantsForPlayFab.PLAYERWSHIELDID => _playerwithshieldSprite
        };

        PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest
        {
            CharacterId = characterResult.CharacterId
        }, OnGetCharacterData, OnError);
    }

    private void OnGetCharacterData(GetCharacterStatisticsResult result)
    {
        _hpText.text = "HP: " + result.CharacterStatistics[ConstantsForPlayFab.PLAYERHEALTH];
        _dmgText.text = "DMG: " + result.CharacterStatistics[ConstantsForPlayFab.PLAYERDAMAGE];
    }

    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.Log(errorMessage);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickCharacterView?.Invoke(this);
    }
    public void SelectView()
    {
        IsSelected = true;
        _backGroundImage.color = _selectedColor;
    }

    public void DeselectView()
    {
        IsSelected = false;
        _backGroundImage.color = _idleColor;
    }
}
