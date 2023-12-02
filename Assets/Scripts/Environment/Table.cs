using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Table : MonoBehaviour, IUseable
{
    [SerializeField] private float _itemMass;

    private BoxCollider2D _boxColleder;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private bool _isItemUsed = false;

    private void Awake()
    {
        _boxColleder = GetComponent<BoxCollider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    public void UseItem()
    {
        if (!_isItemUsed)
        {
            Bounds bounds = _boxColleder.bounds;

            Vector3 closestPointToPlayer = bounds.ClosestPoint(GameManager.Instance.GetPlayer.GetPlayerPosition);

            if(closestPointToPlayer.x == bounds.max.x)
            {
                _animator.SetBool(Settings.flipLeft, true);
            }
            else if (closestPointToPlayer.x == bounds.min.x)
            {
                _animator.SetBool(Settings.flipRight, true);
            }
            else if (closestPointToPlayer.y == bounds.min.y)
            {
                _animator.SetBool(Settings.flipUp, true);
            }
            else if (closestPointToPlayer.y == bounds.max.y)
            {
                _animator.SetBool(Settings.flipDown, true);
            }

            gameObject.layer = LayerMask.NameToLayer("Environment");

            _rigidbody.mass = _itemMass;

            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.tableFlip);

            _isItemUsed = true;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(_itemMass), _itemMass, false);
    }
#endif
}
