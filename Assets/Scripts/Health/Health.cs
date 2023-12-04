using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthBar _healthBar;

    private int _startingHealth;
    private int _currentHealth;
    private HealthEvent _healthEvent;
    private Player _player;
    private Coroutine _immunityCoroutine;
    private bool _isImmuneAfterHit = false;
    private float _immunityTime = 0f;
    private SpriteRenderer _spriteRenderer = null;
    private const float _spriteFlashInterval = 0.2f;
    private WaitForSeconds _waitForSecondsSpriteFlashInterval =
        new WaitForSeconds(_spriteFlashInterval);

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

        if (_player != null)
        {
            InitializeImmunity(_player.PlayerDetails.isImmuneAfterHit, 
                _player.PlayerDetails.hitImmunityTime, 
                _player.PlayerSpriteRenderer);

            return;
        }

        if (EnemyReference != null)
        {
            InitializeImmunity(EnemyReference.EnemyDetails.IsImmunetAfetrHit, 
                EnemyReference.EnemyDetails.HitImmunityTime, 
                EnemyReference.SpriteRendererArray[0]);

            _healthBar?.DisableHealthBar();

            if (EnemyReference.EnemyDetails.IsHealthBarDisplayed)
                _healthBar?.EnableHealthBar();
        }
    }

    private void InitializeImmunity(bool isImmuneAfterHit, 
        float immunityTime, 
        SpriteRenderer spriteRenderer)
    {
        if (isImmuneAfterHit)
        {
            _isImmuneAfterHit = true;
            _immunityTime = immunityTime;
            _spriteRenderer = spriteRenderer;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        bool isRolling = false;

        if (_player != null)
            isRolling = _player.PlayerControlReference.IsPlayerRolling;

        if (isRolling)
            return;
   
        if (!IsDamageable)
            return;

         _currentHealth -= damageAmount;
         CallHealthEvent(damageAmount);

         PostHitImmunity();  

        if(_healthBar != null && EnemyReference.EnemyDetails.IsHealthBarDisplayed == true)
        {
            _healthBar.SetHealthBarValue((float)_currentHealth / (float)_startingHealth);
        }
    }

    private void PostHitImmunity()
    {
        if (gameObject.activeSelf == false)
            return;

        if (_isImmuneAfterHit)
        {
            if (_immunityCoroutine != null)
                StopCoroutine(_immunityCoroutine);

            _immunityCoroutine = StartCoroutine(PostHitImmunityRoutine(_immunityTime, _spriteRenderer));
        }
    }

    private IEnumerator PostHitImmunityRoutine(float immunityTime, SpriteRenderer spriteRenderer)
    {
        int iterations = Mathf.RoundToInt(immunityTime / _spriteFlashInterval / 2f);

        IsDamageable = false;

        while(iterations > 0)
        {
            spriteRenderer.color = Color.red;

            yield return _waitForSecondsSpriteFlashInterval;

            spriteRenderer.color = Color.white;

            yield return _waitForSecondsSpriteFlashInterval;

            iterations--;

            yield return null;
        }

        IsDamageable = true;
    }

    private void CallHealthEvent(int damageAmount)
    {
        _healthEvent.CallHealthChangedEvent((float)_currentHealth / (float)_startingHealth, _currentHealth, damageAmount);
    }

    public void AddHealth(int healthPercent)
    {
        int healthIncrease = Mathf.RoundToInt(_startingHealth * healthPercent / 100f);

        int totalHealth = _currentHealth + healthPercent;

        if(totalHealth > _startingHealth)
        {
            _currentHealth = _startingHealth;
        }

        _currentHealth = totalHealth;

        CallHealthEvent(0);
    }
}
