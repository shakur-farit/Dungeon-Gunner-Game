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
           // {ChestState.ammoItem, () => CollectAmmoItem() },
            //{ChestState.weaponItem, () => CollectWeaponItem() },
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

    private void UpdateChestState()
    {
        if (_healthPrecent != 0)
        {
            _chestState = ChestState.healthItem;
            IntantiateHealthItem();
        }
        else if (_ammoPrecent != 0)
        {
            _chestState = ChestState.ammoItem;
            IntantiateAmmoItem();
        }
        else if (_weaponDetails != null)
        {
            _chestState = ChestState.weaponItem;
            IntantiateWeaponItem();
        }
        else
        {
            _chestState = ChestState.empty;
        }
    }

    private void IntantiateHealthItem()
    {
        InstantiateItem();
        _chestItem.Initialize(GameResources.Instance.heartIcon, _healthPrecent.ToString() + "%",
            _itemSpawnPoint.position, _materializeColor);
    }

    private void IntantiateAmmoItem()
    {
        throw new NotImplementedException();
    }

    private void IntantiateWeaponItem()
    {
        throw new NotImplementedException();
    }

    private void InstantiateItem()
    {
        _chestItemGameObject = Instantiate(GameResources.Instance.chestItemPrefab, transform);

        _chestItem = _chestItemGameObject.GetComponent<ChestItem>();
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
}
