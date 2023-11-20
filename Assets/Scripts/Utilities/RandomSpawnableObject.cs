using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnableObject<T>
{
    private struct _chanceBoundaries
    {
        public T SpawnableObject;
        public int LowBoundaryValue;
        public int HighBoundaryValue;
    }

    private int _ratioValueTotal = 0;
    private List<_chanceBoundaries> _chanceBoundariesList = new List<_chanceBoundaries>();
    private List<SpawnableObjectsByLevel<T>> _spawnableObjectsByLevelList;

    public RandomSpawnableObject(List<SpawnableObjectsByLevel<T>> spawnableObjectsByLevelList)
    {
        _spawnableObjectsByLevelList = spawnableObjectsByLevelList;
    }

    public T GetItem()
    {
        int upperBoundary = -1;
        _ratioValueTotal = 0;
        _chanceBoundariesList.Clear();
        T spawnableObject = default(T);

        foreach (SpawnableObjectsByLevel<T> spawnableObjectsByLevel in _spawnableObjectsByLevelList)
        {
            if(spawnableObjectsByLevel.DungeonLevel == GameManager.Instance.getCurrentDungeonLevel)
            {
                foreach (SpawnableObjectRatio<T> spawnableObjectRatio in spawnableObjectsByLevel.SpawnableObjectRatioList)
                {
                    int lowerBoundary = upperBoundary + 1;

                    upperBoundary = lowerBoundary + spawnableObjectRatio.Ratio - 1;

                    _ratioValueTotal += spawnableObjectRatio.Ratio;

                    _chanceBoundariesList.Add(new _chanceBoundaries()
                    {
                        SpawnableObject = spawnableObjectRatio.DungeonObject,
                        LowBoundaryValue = lowerBoundary,
                        HighBoundaryValue = upperBoundary
                    });
                }
            }
        }

        if (_chanceBoundariesList.Count == 0)
            return default(T);

        int lookUpValue = Random.Range(0, _ratioValueTotal);

        foreach (_chanceBoundaries spawnChance in _chanceBoundariesList)
        {
            if(lookUpValue >=spawnChance.LowBoundaryValue && lookUpValue <= spawnChance.HighBoundaryValue)
            {
                spawnableObject = spawnChance.SpawnableObject;
                break;
            }
        }

        return spawnableObject;
    }
}
