using System;

public static class StaticEventHandler
{
    public static event Action<RoomChangedEventArgs> OnRoomChanged;

    public static void CallRoomChangedEvent(Room room)
    {
        OnRoomChanged?.Invoke(new RoomChangedEventArgs()
        {
            Room = room
        });
    }
}

public class RoomChangedEventArgs : EventArgs
{
    public Room Room;
}
