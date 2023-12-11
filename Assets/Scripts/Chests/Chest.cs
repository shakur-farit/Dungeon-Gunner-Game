using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MaterializeEffect))]
public class Chest : MonoBehaviour, IUseable
{
    [ColorUsage(false, true)]
    [SerializeField] private Color _materializeColor;
    [SerializeField] private float _materializeTime = 3f;
    [SerializeField] private Transform _itemSpawnPoint;

    private int _healthPrecent;
    private WeaponDetailsSO _weaponDetails;
    private int _ammoPrecent;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private MaterializeEffect _materializeEffect;
    private bool _isEnabled = false;
    private ChestState _chestState = ChestState.closed;
    private GameObject _chestItemGameObject;
    private ChestItem _chestItem;
    private TextMeshPro _messageText;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _materializeEffect = GetComponent<MaterializeEffect>();
        _messageText = GetComponentInChildren<TextMeshPro>();
    }

    public void Initialize(bool shouldMaterialize, int healthPrecent,
        WeaponDetailsSO weaponDetails, int ammoPrecent)
    {
        _healthPrecent = healthPrecent;
        _weaponDetails = weaponDetails;
        _ammoPrecent = ammoPrecent;

        if (shouldMaterialize)
        {
            StartCoroutine(MaterializeChest());
            return;
        }

        EnableChest();
    }

    private IEnumerator MaterializeChest()
    {
        SpriteRenderer[] spriteRenderersArray = new SpriteRenderer[]
        {
            _spriteRenderer
        };

        yield return StartCoroutine(_materializeEffect
            .MaterializeRoutine(GameResources.Instance.materializeShader,
            _materializeColor, _materializeTime, spriteRenderersArray,
            GameResources.Instance.litMaterial));

        EnableChest();
    }

    private void EnableChest()
    {
        _isEnabled = true;
    }

    public void UseItem()
    {
        if (!_isEnabled)
            return;

        Dictionary<ChestState, Action> chestAction = new Dictionary<ChestState, Action>()
        {
            {ChestState.closed, () => OpenChest() },
            {ChestState.healthItem, () => CollectHealthItem() },
            {ChestState.ammoItem, () => CollectAmmoItem() },
            {ChestState.weaponItem, () => CollectWeaponItem() },
            {ChestState.empty, null }
        };

        if (chestAction.TryGetValue(_chestState, out Action action))
        {
            action();
        }
    }

    private void OpenChest()
    {
        _animator.SetBool(Settings.use, true);

        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.chestOpen);

        if (_weaponDetails != null)
        {
            if (GameManager.Instance.GetPlayer.IsWeaponHeldByPlayer(_weaponDetails))
                _weaponDetails = null;
        }

        UpdateChestState();
    }
    private void CollectHealthItem()
    {
        if (_chestItem == null || !_chestItem.IsItemmaterialized)
            return;

        GameManager.Instance.GetPlayer.PlayerHealth.AddHealth(_healthPrecent);

        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.healthPickup);

        _healthPrecent = 0;

        Destroy(_chestItemGameObject);

        UpdateChestState();
    }

    private void CollectAmmoItem()
    {
        if (_chestItem == null || !_chestItem.IsItemmaterialized)
            return;

        Player player = GameManager.Instance.GetPlayer;

        player.PlayerReloadWeaponEvent
            .CallOnRealoadWeaponEvent(player.PlayerActiveWeapon.GetCurrentWeapon, _ammoPrecent);

        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.ammoPickup);

        _ammoPrecent = 0;

        Destroy(_chestItemGameObject);

        UpdateChestState();
    }

    private void CollectWeaponItem()
    {
        if (_chestItem == null || !_chestItem.IsItemmaterialized)
            return;

        if (!GameManager.Instance.GetPlayer.IsWeaponHeldByPlayer(_weaponDetails))
        {
            GameManager.Instance.GetPlayer.AddWeaponToPlayer(_weaponDetails);

            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.weaponPickup);
        }
        else
        {
            StartCoroutine(DisplayMessage("WEAPON\nALREADY\nEQUIPPED", 5f));
        }

        _weaponDetails = null;

        Destroy(_chestItemGameObject);

        UpdateChestState();
    }

    private void UpdateChestState()
    {
        var chestActions = new Dictionary<Func<bool>, (ChestState, Action)>
        {
            { () => _healthPrecent != 0, (ChestState.healthItem, InstantiateHealthItem) },
            { () => _ammoPrecent != 0, (ChestState.ammoItem, InstantiateAmmoItem) },
            { () => _weaponDetails != null, (ChestState.weaponItem, InstantiateWeaponItem) }
        };

        foreach (var entry in chestActions)
        {
            if (entry.Key())
            {
                _chestState = entry.Value.Item1;
                entry.Value.Item2();
                return;
            }
        }

        _chestState = ChestState.empty;
    }

    private void InstantiateHealthItem()
    {
        InstantiateItem();
        _chestItem.Initialize(GameResources.Instance.heartIcon, _healthPrecent.ToString() + "%",
            _itemSpawnPoint.position, _materializeColor);
    }

    private void InstantiateAmmoItem()
    {
        InstantiateItem();

        _chestItem.Initialize(GameResources.Instance.bulletIcon, 
            _ammoPrecent.ToString() + "%", _itemSpawnPoint.position,
            _materializeColor);
    }

    private void InstantiateWeaponItem()
    {
        InstantiateItem();

        _chestItemGameObject.GetComponent<ChestItem>().Initialize(_weaponDetails.weaponSprite,
            _weaponDetails.weaponName, _itemSpawnPoint.position, _materializeColor);
    }

    private void InstantiateItem()
    {
        _chestItemGameObject = Instantiate(GameResources.Instance.chestItemPrefab, transform);

        _chestItem = _chestItemGameObject.GetComponent<ChestItem>();
    }

    private IEnumerator DisplayMessage(string messageText, float messageDisplayTime)
    {
        _messageText.text = messageText;

        yield return new WaitForSeconds(messageDisplayTime);

        _messageText.text = string.Empty;
    }
}
