using UnityEngine;

[DisallowMultipleComponent]
public class DealContactDamage : MonoBehaviour
{
    [Header("Deal Damage")]
    [SerializeField] private int _contactDamageAmount;
    [SerializeField] private LayerMask _layerMask;

    private bool _isColliding = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isColliding)
            return;

        ContactDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_isColliding) 
            return;

        ContactDamage(collision);
    }

    private void ContactDamage(Collider2D collision)
    {
        int collisionObjectLayerMask = (1 << collision.gameObject.layer);

        if ((_layerMask.value & collisionObjectLayerMask) == 0)
            return;

        ReceiveContactDamage receiveContactDamage = collision.gameObject.GetComponent<ReceiveContactDamage>();

        if(receiveContactDamage != null)
        {
            _isColliding = true;

            Invoke("ResetContactCollision", Settings.contancDamageCollisionResetDellay);

            receiveContactDamage.TakeContactDamage(_contactDamageAmount);
        }
    }

    private void ResetContactCollision()
    {
        _isColliding = false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(_contactDamageAmount), _contactDamageAmount, true);
    }
#endif
}
