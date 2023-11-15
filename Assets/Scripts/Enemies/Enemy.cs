using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent (typeof(SortingGroup))]
[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(CircleCollider2D))]
[RequireComponent (typeof(PolygonCollider2D))]
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemyDetailsSO EnemyDetails;
    [HideInInspector] public SpriteRenderer[] SpriteRendererArray;

    private CircleCollider2D _circleCollider2D;
    private PolygonCollider2D _polygonCollider2D;

    private void Awake()
    {
        _circleCollider2D = GetComponent<CircleCollider2D>();
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
        SpriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
    }
}
