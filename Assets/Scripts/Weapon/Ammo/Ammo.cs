using UnityEngine;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable
{
    [SerializeField] private TrailRenderer _trailRenderer;

    private float _ammoRange = 0f; // range of each ammo
    private float _ammoSpeed = 3f;
    private Vector3 _fireDirectionVector;
    private float _fireDirectionAngle;
    private SpriteRenderer _spriteRenderer;
    private AmmoDetailsSO _ammoDetails;
    private float _ammoChargeTimer;
    private bool _isAmmoMaterialSet = false;
    private bool _ovverideAmmoMovement;
    private bool _isColliding;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(_ammoChargeTimer > 0f)
        {
            _ammoChargeTimer -= Time.deltaTime;
            return;
        }

        if (!_isAmmoMaterialSet)
        {
            SetAmmoMaterial(_ammoDetails.ammoMaterial);
            _isAmmoMaterialSet = true;
        }

        Vector3 distanceVector = _fireDirectionVector * _ammoSpeed * Time.deltaTime;

        transform.position += distanceVector;

        _ammoRange -= distanceVector.magnitude;

        if (_ammoRange < 0)
        {
            if (_ammoDetails.isPlayerAmmo)
            {
                StaticEventHandler.CallMultiplierEvent(false);
            }

            DisableAmmo();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isColliding)
            return;

        DealDamage(collision);

        PlayAmmoHitEffect();

        DisableAmmo();
    }

    private void DealDamage(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();

        bool enemyHit = false;

        if (health != null)
        {
            _isColliding = true;
            health.TakeDamage(_ammoDetails.ammoDamage);

            if(health.EnemyReference != null)
            {
                enemyHit = true;
            }
        }

        if (_ammoDetails.isPlayerAmmo)
        {
            if (enemyHit)
            {
                StaticEventHandler.CallMultiplierEvent(true);
                return;
            }

            StaticEventHandler.CallMultiplierEvent(false);
        }
    }
    
    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle,
        float ammoSpeed, Vector3 weaponAimDirectionVector, bool ovverideAmmoMovement = false)
    {
        _ammoDetails = ammoDetails;

        _isColliding = false;

        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        _spriteRenderer.sprite = ammoDetails.ammoSprite;

        _ammoChargeTimer = ammoDetails.ammoChargeTime > 0f ? ammoDetails.ammoChargeTime : 0f;
        _isAmmoMaterialSet = _ammoChargeTimer > 0f ? false : true;
        SetAmmoMaterial(_isAmmoMaterialSet ? ammoDetails.ammoMaterial : ammoDetails.ammoChargeMaterial);

        _ammoRange = ammoDetails.ammoRange;
        _ammoSpeed = ammoSpeed;
        _ovverideAmmoMovement = ovverideAmmoMovement;

        gameObject.SetActive(true);

        _trailRenderer.emitting = ammoDetails.isAmmoTrail;
        _trailRenderer.gameObject.SetActive(ammoDetails.isAmmoTrail);

        if (ammoDetails.isAmmoTrail)
        {
            _trailRenderer.material = ammoDetails.ammoTrailMaterial;
            _trailRenderer.startWidth = ammoDetails.ammoStartTrailWidth;
            _trailRenderer.endWidth = ammoDetails.ammoEndTrailWidth;
            _trailRenderer.time = ammoDetails.ammoTrailTime;
        }
    }

    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, 
        float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        float randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

        int spreadToogle = Random.Range(0, 2) * 2 - 1;

        _fireDirectionAngle = weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance
           ? aimAngle
           : weaponAimAngle;

        _fireDirectionAngle += spreadToogle * randomSpread;

        transform.eulerAngles = new Vector3(0f, 0f, _fireDirectionAngle);

        _fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(_fireDirectionAngle);
    }

    private void PlayAmmoHitEffect()
    {
        if(_ammoDetails.ammoHitEffect != null &&
            _ammoDetails.ammoHitEffect.AmmoHitEffectPrefab != null)
        {
            AmmoHitEffect ammoHitEffect = (AmmoHitEffect)PoolManager
                .Instance.ReuseComponent(_ammoDetails.ammoHitEffect.AmmoHitEffectPrefab,
                transform.position, Quaternion.identity);

            ammoHitEffect.SetHitEffect(_ammoDetails.ammoHitEffect);

            ammoHitEffect.gameObject.SetActive(true);
        }
    }

    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    private void SetAmmoMaterial(Material material)
    {
        _spriteRenderer.material = material;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(_trailRenderer), _trailRenderer);
    }
#endif
}
