using UnityEngine;

public class AmmoPatern : MonoBehaviour, IFireable
{
    [SerializeField] private Ammo[] _ammoArray;

    private float _ammoRange;
    private float _ammoSpeed;
    private Vector3 _fireDirectionVector;
    private float _fireDirectionAngle;
    private AmmoDetailsSO _ammoDetails;
    private float _ammoChargeTime;

    private void Update()
    {
        if(_ammoChargeTime > 0f)
        {
            _ammoChargeTime -= Time.deltaTime;
            return;
        }

        Vector3 distanceVector = _fireDirectionVector * _ammoSpeed * Time.deltaTime;

        transform.position += distanceVector;

        transform.Rotate(new Vector3(0f, 0f, _ammoDetails.ammoRotationSpeed * Time.deltaTime));

        _ammoRange -= distanceVector.magnitude;

        if(_ammoRange < 0f)
        {
            DisableAmmo();
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, 
        float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, 
        bool ovverideAmmoMovement = false)
    {
        _ammoDetails = ammoDetails;
        _ammoSpeed = ammoSpeed;

        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        _ammoRange = ammoDetails.ammoRange;

        gameObject.SetActive(true);

        foreach (Ammo ammo in _ammoArray)
        {
            ammo.InitialiseAmmo(ammoDetails, aimAngle, weaponAimAngle, 
                ammoSpeed, weaponAimDirectionVector, true);
        }

        if(ammoDetails.ammoChargeTime > 0f)
        {
            _ammoChargeTime = ammoDetails.ammoChargeTime;
            return;
        }

        _ammoChargeTime = 0f;
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

        _fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(_fireDirectionAngle);
    }

    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(_ammoArray), _ammoArray);
    }
#endif
}
