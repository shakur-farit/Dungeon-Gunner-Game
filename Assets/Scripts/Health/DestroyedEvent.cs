using System;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyedEvent : MonoBehaviour
{
    public event Action<DestroyedEvent, DestoryedEventArgs> OnDestoryedEvent;

    public void CallDestroyedEvent(bool playerDied, int points)
    {
        OnDestoryedEvent?.Invoke(this, new DestoryedEventArgs()
        {
            PlayerDied = playerDied,
            Points = points
        });
    }
}

public class DestoryedEventArgs : EventArgs
{
    public bool PlayerDied;
    public int Points;
}
