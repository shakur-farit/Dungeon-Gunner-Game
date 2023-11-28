using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [Header("Game Object References")]
    [SerializeField] private GameObject _healthBar;

    public void EnableHealthBar()
    {
        gameObject.SetActive(true);
    }

    public void DisableHealthBar()
    {
        gameObject.SetActive(false);
    }

    public void SetHealthBarValue(float healthPrecent)
    {
        _healthBar.transform.localScale = new Vector3(healthPrecent, 1f, 1f);
    }
}
