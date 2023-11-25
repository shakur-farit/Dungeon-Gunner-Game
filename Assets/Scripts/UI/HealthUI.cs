using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthUI : MonoBehaviour
{
    private List<GameObject> _healthHeartList = new List<GameObject>();

    private void OnEnable()
    {
        GameManager.Instance.GetPlayer.PlayerHealthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.GetPlayer.PlayerHealthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        SetHealthBar(healthEventArgs);
    }

    private void ClearHealthBar()
    {
        foreach (GameObject heartIcon in _healthHeartList)
        {
            Destroy(heartIcon);
        }

        _healthHeartList.Clear();
    }

    private void SetHealthBar(HealthEventArgs healthEventArgs)
    {
        ClearHealthBar();

        int heartSpriteAmount = GameResources.Instance.currentPlayer.playerDetails.playerHealthAmount;

        int healthHearts = Mathf.CeilToInt(healthEventArgs.HealthPercent * heartSpriteAmount / 20);

        for (int i = 0; i < healthHearts; i++)
        {
            GameObject heart = Instantiate(GameResources.Instance.heartPrefab, transform);

            heart.GetComponent<RectTransform>().anchoredPosition = new Vector2(Settings.uiHeartSpacing * i, 0f);

            _healthHeartList.Add(heart);
        }
    }
}
