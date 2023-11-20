using System.Collections.Generic;

[System.Serializable]
public class SpawnableObjectsByLevel<T>
{
    public DungeonLevelSO DungeonLevel;
    public List<SpawnableObjectRatio<T>> SpawnableObjectRatioList;
}
