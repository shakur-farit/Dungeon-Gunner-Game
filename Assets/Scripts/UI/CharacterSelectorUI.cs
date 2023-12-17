using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[DisallowMultipleComponent]
public class CharacterSelectorUI : MonoBehaviour
{
    [SerializeField] private Transform _characterSelector;
    [SerializeField] private TMP_InputField _playerNameInput;

    public List<PlayerDetailsSO> _playerDetailsList;
    private GameObject _playerSelectionPrefab;
    private CurrentPlayerSO _currentPlayer;
    private List<GameObject> _playerCharacterGameObjectList = new List<GameObject>();
    private Coroutine _coroutine;
    private int _selectedPlayerIndex = 0;
    private float _offset = 4f;

    private void Awake()
    {
        _playerSelectionPrefab = GameResources.Instance.playerSelectionPrefab;
        _playerDetailsList = GameResources.Instance.playerDetailsList;
        _currentPlayer = GameResources.Instance.currentPlayer;
    }

    private void Start()
    {
        for (int i = 0; i < _playerDetailsList.Count; i++)
        {
            GameObject playerSelectionObject = Instantiate(_playerSelectionPrefab, _characterSelector);
            _playerCharacterGameObjectList.Add(playerSelectionObject);
            playerSelectionObject.transform.localPosition = new Vector3((_offset * i), 0f, 0f);
            PopulatePlayerDetails(playerSelectionObject.GetComponent<PlayerSelectionUI>(), _playerDetailsList[i]);
        }

        _playerNameInput.text = _currentPlayer.playerName;
        _currentPlayer.playerDetails = _playerDetailsList[_selectedPlayerIndex];
    }

    private void PopulatePlayerDetails(PlayerSelectionUI playerSelectionUI, PlayerDetailsSO playerDetailsSO)
    {
        playerSelectionUI.PlayerHandSpriteRenderer.sprite = playerDetailsSO.playerHandSprite;
        playerSelectionUI.PlayerHandNoWeaponSpriteRenderer.sprite = playerDetailsSO.playerHandSprite;
        playerSelectionUI.PlayerWeaponSpriteRenderer.sprite = playerDetailsSO.startingWeapon.weaponSprite;
        playerSelectionUI.PlayerAnimator.runtimeAnimatorController = playerDetailsSO.runtimeAnimatorController;
    }

    public void NextChaacter()
    {
        if (_selectedPlayerIndex >= _playerDetailsList.Count - 1)
            return;

        _selectedPlayerIndex++;
        _currentPlayer.playerDetails = _playerDetailsList[_selectedPlayerIndex];

        MoveToSelectedCharacter(_selectedPlayerIndex);
    }

    public void PreviousCharacter()
    {
        if (_selectedPlayerIndex == 0)
            return;

        _selectedPlayerIndex--;
        _currentPlayer.playerDetails = _playerDetailsList[_selectedPlayerIndex];

        MoveToSelectedCharacter(_selectedPlayerIndex);
    }

    private void MoveToSelectedCharacter(int selectedPlayerIndex)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(MoveToSelectedCharacterRoutine(selectedPlayerIndex));
    }

    private IEnumerator MoveToSelectedCharacterRoutine(int index)
    {
        float currentLocalXPosition = _characterSelector.localPosition.x;
        float targetLocalXPosotion = index * _offset * _characterSelector.localScale.x * -1f;

        while (Mathf.Abs(currentLocalXPosition - targetLocalXPosotion) > 0.01f)
        {
            currentLocalXPosition = Mathf.Lerp(currentLocalXPosition, targetLocalXPosotion, Time.deltaTime * 10f);
            _characterSelector.localPosition = new Vector3(currentLocalXPosition, _characterSelector.localPosition.y, 0f);
            yield return null;
        }

        _characterSelector.localPosition = new Vector3(targetLocalXPosotion, _characterSelector.localPosition.y, 0f);
    }

    public void UpdatePlayerName()
    {
        _playerNameInput.text = _playerNameInput.text.ToUpper();

        _currentPlayer.playerName = _playerNameInput.text;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(_characterSelector), _characterSelector);
        HelperUtilities.ValidateCheckNullValue(this, nameof(_playerNameInput), _playerNameInput);
    }
#endif
}
