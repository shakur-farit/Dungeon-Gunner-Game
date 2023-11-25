using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int _startingHealth;
    private int _currentHealth;
    private HealthEvent _healthEvent;
    private Player _player;

    [HideInInspector] public bool IsDamageable = true;
    [HideInInspector] public Enemy EnemyReference;

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

        _player = GetComponent<Player>();
        EnemyReference = GetComponent<Enemy>();

    }

    public void TakeDamage(int damageAmount)
    {
        if (_player == null)
            return;

        if (_player.PlayerControlReference.IsPlayerRolling)
        {
            Debug.Log("Dodging the damage");
            return;
        }

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
