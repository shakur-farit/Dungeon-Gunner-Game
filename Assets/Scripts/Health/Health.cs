using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int _startingHealth;
    private int _currentHealth;
    private HealthEvent _healthEvent;

    [HideInInspector] public bool IsDamageable = true;

    public int GetStartingHealth => _startingHealth;

    public int SetStartingHealth
    {
        set 
        { 
            _startingHealth = value;
            _currentHealth = value;
        }
    }

    private void Awake()
    {
        _healthEvent = GetComponent<HealthEvent>();
    }

    private void Start()
    {
        CallHealthEvent(0);
    }

    public void TakeDamage(int damageAmount)
    {
        if (IsDamageable)
        {
            _currentHealth -= damageAmount;
            CallHealthEvent(damageAmount);
        }
    }

    private void CallHealthEvent(int damageAmount)
    {
        _healthEvent.CallHealthChangedEvent(((float)_currentHealth / (float)_startingHealth), _currentHealth, damageAmount);
    }
}
