using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]    
public class MoveItem : MonoBehaviour
{
    [Header("Sound Effect")]
    [SerializeField] private SoundEffectSO _moveSoundEffect;

    [HideInInspector] public BoxCollider2D ItemBoxCollieder;

    private Rigidbody2D _rigidbody;
    private InstantiatedRoom _room;
    private Vector3 _previousPosition;

    private void Awake()
    {
        ItemBoxCollieder = GetComponent<BoxCollider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _room = GetComponentInParent<InstantiatedRoom>();

        _room.moveableItemsList.Add(this);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        UpdateObstacles();
    }

    private void UpdateObstacles()
    {
        ConfineItemRoomBounds();

        _room.UpdateMoveableObstacles();

        _previousPosition = transform.position;

        if(Mathf.Abs(_rigidbody.velocity.x) > 0.001f || Mathf.Abs(_rigidbody.velocity.y) > 0.001f)
        {
            if(_moveSoundEffect != null && Time.frameCount % 10 == 0)
            {
                SoundEffectManager.Instance.PlaySoundEffect(_moveSoundEffect);
            }
        }
    }

    private void ConfineItemRoomBounds()
    {
        Bounds itemBounds = ItemBoxCollieder.bounds;
        Bounds roomBounds = _room.roomColliderBounds;

        if(itemBounds.min.x <= roomBounds.min.x ||
           itemBounds.max.x >= roomBounds.max.x ||
           itemBounds.min.y <= roomBounds.min.y ||
           itemBounds.max.y >= roomBounds.max.y)
        {
            transform.position = _previousPosition;
        }
    }
}
