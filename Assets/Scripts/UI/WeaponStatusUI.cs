using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class WeaponStatusUI : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private Image _weaponImage;
    [SerializeField] private Transform _ammoHolderTransfrom;
    [SerializeField] private TextMeshProUGUI _reloadText;
    [SerializeField] private TextMeshProUGUI _ammoRemainingText;
    [SerializeField] private TextMeshProUGUI _weaponNameText;
    [SerializeField] private Transform _reloadBar;
    [SerializeField] private Image _barImage;

    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkinReloadTextCoroutine;

    private void Awake()
    {
        player = GameManager.Instance.GetPlayer;
    }

    private void OnEnable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;

        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;

        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;

        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void OnDisable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;

        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;

        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;

        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void Start()
    {
        SetActiveWeapon(player.activeWeapon.GetCurrentWeapon);
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, 
        SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetActiveWeapon(setActiveWeaponEventArgs.Weapon);
    }

    private void SetActiveWeapon(Weapon weapon)
    {
        UpdateActiveWeaponImage(weapon.WeaponDetails);
        UpdateActiveWeaponName(weapon);
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);

        if (weapon.IsWeaponReloading)
        {
            UpdateWeaponReloadBar(weapon);
        }
        else
        {
            ResetWeaponReloadBar();
        }

        UpdateReloadText(weapon);
    }

    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, 
        WeaponFiredEventArgs weaponFiredEventArgs)
    {
        WeaponFired(weaponFiredEventArgs.Weapon);
    }

    private void WeaponFired(Weapon weapon)
    {
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
        UpdateReloadText(weapon);
    }

    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, 
        ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        UpdateWeaponReloadBar(reloadWeaponEventArgs.Weapon);
    }

    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent,
        WeaponFiredEventArgs weaponFiredEventArgs)
    {
        WeaponReloaded(weaponFiredEventArgs.Weapon);
    }

    private void WeaponReloaded(Weapon weapon)
    {
       if(player.activeWeapon.GetCurrentWeapon == weapon)
       {
           UpdateReloadText(weapon);
           UpdateAmmoText(weapon);
           UpdateAmmoLoadedIcons(weapon);
           ResetWeaponReloadBar();
       }
    }

    private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetails)
    {
        _weaponImage.sprite = weaponDetails.weaponSprite;
    }

    private void UpdateActiveWeaponName(Weapon weapon)
    {
        _weaponNameText.text = "(" + weapon.WeaponListPosition + ")" + 
            weapon.WeaponDetails.weaponName.ToString();
    }

    private void UpdateAmmoText(Weapon weapon)
    {
        if (weapon.WeaponDetails.hasInfiniteAmmo)
        {
            _ammoRemainingText.text = "INFINITE AMMO";
            return;
        }

        _ammoRemainingText.text = weapon.WeaponRemainingAmmo.ToString() + " / " +
            weapon.WeaponDetails.weaponAmmoCapacity.ToString();
    }

    private void UpdateAmmoLoadedIcons(Weapon weapon)
    {
        ClearAmmoLoadedIcons();

        for (int i = 0; i < weapon.WeaponClipRemainingAmmo; i++)
        {
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, _ammoHolderTransfrom);

            ammoIcon.GetComponent<RectTransform>().anchoredPosition = 
                new Vector2(0f, Settings.uiAmmoIconSpacing * i);

            ammoIconList.Add(ammoIcon);
        }
    }

    private void ClearAmmoLoadedIcons()
    {
        foreach (GameObject ammoIcon in ammoIconList)
        {
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();
    }

    private void UpdateWeaponReloadBar(Weapon weapon)
    {
        if (weapon.WeaponDetails.hasInfinityClipCapacity)
            return;

        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);

        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
    }

    private void ResetWeaponReloadBar()
    {
        StopReloadWeaponCoroutine();

        _barImage.color = Color.green;

        _reloadBar.transform.localScale = new Vector3(1f,1f,1f);
    }

    private void StopReloadWeaponCoroutine()
    {
        if (reloadWeaponCoroutine != null)
            StopCoroutine(reloadWeaponCoroutine);
    }

    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon weapon)
    {
        _barImage.color = Color.red;

        while (weapon.IsWeaponReloading)
        {
            
            float barFill = weapon.WeaponReloadTimer / weapon.WeaponDetails.weaponRealoadTime;

            _reloadBar.transform.localScale = new Vector3(barFill, 1f, 1f);

            yield return null;
        }
    }

    private void UpdateReloadText(Weapon weapon)
    {
        if((!weapon.WeaponDetails.hasInfinityClipCapacity) &&
            (weapon.WeaponClipRemainingAmmo <= 0 || weapon.IsWeaponReloading))
        {
            _barImage.color = Color.red;

            StopBlinkingReloadTextCoroutine();

            blinkinReloadTextCoroutine = StartCoroutine(StopBlinkingReloadTextRoutine());

            return;
        }

        StopBlinkingReloadText();
    }

    private void StopBlinkingReloadTextCoroutine()
    {
        if (blinkinReloadTextCoroutine != null)
            StopCoroutine(blinkinReloadTextCoroutine);
    }

    private IEnumerator StopBlinkingReloadTextRoutine()
    {
        while (true)
        {
            _reloadText.text = "RELOAD";
            yield return new WaitForSeconds(.3f);
            _reloadText.text = "";
            yield return new WaitForSeconds(.3f);
        }
    }

    private void StopBlinkingReloadText()
    {
        StopBlinkingReloadTextCoroutine();

        _reloadText.text = "";
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(_weaponImage), _weaponImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(_ammoHolderTransfrom), _ammoHolderTransfrom);
        HelperUtilities.ValidateCheckNullValue(this, nameof(_reloadText), _reloadText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(_ammoRemainingText), _ammoRemainingText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(_weaponNameText), _weaponNameText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(_reloadBar), _reloadBar);
        HelperUtilities.ValidateCheckNullValue(this, nameof(_barImage), _barImage);
    }
#endif
}
