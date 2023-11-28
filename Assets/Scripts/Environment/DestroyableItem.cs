using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyableItem : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int _startingHealthAmount = 1;

    [Header("Sound Effect")]
    [SerializeField] private SoundEffectSO _destroySoundEffect;

    private Animator _animator;
    private BoxCollider2D _boxCollider;
    private HealthEvent _healthEvent;
    private Health _health;
    private ReceiveContactDamage _receiveContactDamage;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _healthEvent = GetComponent<HealthEvent>();
        _health = GetComponent<Health>();
        _receiveContactDamage = GetComponent<ReceiveContactDamage>();
    }

    private void OnEnable()
    {
        _healthEvent.OnHealthChanged += CallHealthEvent_OnHealthLost;
    }

    private void OnDisable()
    {
        _healthEvent.OnHealthChanged -= CallHealthEvent_OnHealthLost;
    }

    private void CallHealthEvent_OnHealthLost(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if(healthEventArgs.HealthAmount <= 0f)
        {
            StartCoroutine(PlayAnimation());
        }
    }

    private IEnumerator PlayAnimation()
    {
        Destroy(_boxCollider);

        if(_destroySoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(_destroySoundEffect);
        }

        _animator.SetBool(Settings.destroy, true);

        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName(Settings.stateDestroyed))
        {
            yield return null;
        }

        Destroy(_animator);
        Destroy(_receiveContactDamage);
        Destroy(_health);
        Destroy(_healthEvent);
        Destroy(this);
    }
}
