using UnityEngine;

[RequireComponent (typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyWeaponAI : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Transform _weaponShootPosition;

    private Enemy _enemy;
    private EnemyDetailsSO _enemyDetails;
    private float _firingIntervalTimer;
    private float _firingDurationTimer;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        _enemyDetails = _enemy.EnemyDetails;

        _firingIntervalTimer = WeaponShootInterval();
        _firingDurationTimer = WeaponShootDuration();
    }

    private void Update()
    {
        _firingIntervalTimer -= Time.deltaTime;

        if(_firingIntervalTimer < 0f)
        {
            if (_firingDurationTimer >= 0)
            {
                _firingDurationTimer -= Time.deltaTime;

                FireEnemyWeapon();

                return;
            }

            _firingIntervalTimer = WeaponShootInterval();
            _firingDurationTimer = WeaponShootDuration();
        }
    }

    private float WeaponShootInterval()
    {
        return Random.Range(_enemyDetails.FiringIntervalMin, _enemyDetails.FiringIntervalMax);
    }

    private float WeaponShootDuration()
    {
        return Random.Range(_enemyDetails.FiringDurationMin, _enemyDetails.FiringDurationMax);
    }

    private void FireEnemyWeapon()
    {
        Vector3 playerDirectionVectror = GameManager.Instance.GetPlayer.GetPlayerPosition - transform.position;

        Vector3 weaponDirection = GameManager.Instance.GetPlayer.GetPlayerPosition -
            _weaponShootPosition.position;

        float weaponAngleDegress = HelperUtilities.GetAngelFromVector(weaponDirection);

        float enemyAngleDegress = HelperUtilities.GetAngelFromVector(playerDirectionVectror);

        AimDirection enemyAimDiretion = HelperUtilities.GetAimDerection(enemyAngleDegress);

        _enemy.EnemyAimWeaponEvent.CallAimWeaponEvent(enemyAimDiretion, enemyAngleDegress,
            weaponAngleDegress, weaponDirection);


        if(_enemyDetails.EnenmyWeapon != null)
        {
            float enemyAmmoRange = _enemyDetails.EnenmyWeapon.weaponCurrentAmmo.ammoRange;

            if(playerDirectionVectror.magnitude <= enemyAmmoRange)
            {
                if (_enemyDetails.FiringLineOfSightRiquired &&
                    !IsPlayerInLineOfSight(weaponDirection, enemyAmmoRange))
                    return;

                _enemy.EnemyFireWeaponEvent.CallOnFireWeaponEvent(true, true, enemyAimDiretion,
                    enemyAngleDegress, weaponAngleDegress, weaponDirection);
            }
        }
    }

    private bool IsPlayerInLineOfSight(Vector3 weaponDirection, float enemyAmmoRange)
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(_weaponShootPosition.position,
            (Vector2)weaponDirection, enemyAmmoRange, _layerMask);

        if(raycastHit2D && raycastHit2D.transform.CompareTag(Settings.playerTag))
            return true;

        return false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(_weaponShootPosition),_weaponShootPosition);
    }
#endif
}
