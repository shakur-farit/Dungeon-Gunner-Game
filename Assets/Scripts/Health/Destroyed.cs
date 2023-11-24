using UnityEngine;

[RequireComponent(typeof(DestroyedEvent))]
[DisallowMultipleComponent]
public class Destroyed : MonoBehaviour
{
    private DestroyedEvent _destroyedEvent;

    private void Awake()
    {
        _destroyedEvent = GetComponent<DestroyedEvent>();
    }

    private void OnEnable()
    {
        _destroyedEvent.OnDestoryedEvent += DestroyedEvent_OnDestroyed;
    }

    private void OnDisable()
    {
        _destroyedEvent.OnDestoryedEvent -= DestroyedEvent_OnDestroyed;
    }

    private void DestroyedEvent_OnDestroyed(DestroyedEvent destroyedEvent, DestoryedEventArgs destoryedEventArgs)
    {
        if (destoryedEventArgs.PlayerDied)
        {
            gameObject.SetActive(false);
            return;
        }

        Destroy(gameObject);
    }
}
