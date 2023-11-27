using System;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyedEvent : MonoBehaviour
{
    public event Action<DestroyedEvent, DestoryedEventArgs> OnDestoryed;

    public void CallDestroyedEvent(bool playerDied, int points)
    {
        OnDestoryed?.Invoke(this, new DestoryedEventArgs()
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
