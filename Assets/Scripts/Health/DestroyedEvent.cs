using System;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyedEvent : MonoBehaviour
{
    public event Action<DestroyedEvent> OnDestoryedEvent;

    public void CallDestroyedEvent()
    {
        OnDestoryedEvent?.Invoke(this);
    }
}
