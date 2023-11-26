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

    public static event Action<RoomEnemiesDefeatedArgs> OnRoomEnemiesDefeated;

    public static void CallEnemiesDefeatedEvent(Room room)
    {
        OnRoomEnemiesDefeated?.Invoke(new RoomEnemiesDefeatedArgs()
        {
            Room = room
        });
    }

    public static event Action<PointScoreArgs> OnPointScored;

    public static  void CallPointScoredEvent(int points)
    {
        OnPointScored?.Invoke(new PointScoreArgs()
        {
            Points = points
        });
    }

    public static event Action<ScoreChangedArgs> OnScoreChanged;

    public static void CallScoreChangedEvent(long score)
    {
        OnScoreChanged?.Invoke(new ScoreChangedArgs()
        {
            Score = score
        });
    }
}

public class RoomChangedEventArgs : EventArgs
{
    public Room Room;
}

public class RoomEnemiesDefeatedArgs : EventArgs
{
    public Room Room;
}

public class PointScoreArgs : EventArgs
{
    public int Points;
}

public class ScoreChangedArgs : EventArgs
{
    public long Score;
}
