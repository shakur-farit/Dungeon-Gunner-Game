using System;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyedEvent : MonoBehaviour
{
    public event Action<DestroyedEvent, DestoryedEventArgs> OnDestoryedEvent;

    public void CallDestroyedEvent(bool playerDied)
    {
        OnDestoryedEvent?.Invoke(this, new DestoryedEventArgs()
        {
            PlayerDied = playerDied
        });
    }
}

public class DestoryedEventArgs : EventArgs
{
    public bool PlayerDied;
}
