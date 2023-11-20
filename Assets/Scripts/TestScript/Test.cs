using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{
    public RoomTemplateSO roomTemplate;
    private List<SpawnableObjectsByLevel<EnemyDetailsSO>> spawnableObjectsByLevelList;
    private RandomSpawnableObject<EnemyDetailsSO> randomSpawnableObject;
    private GameObject instEnemy;

    private void Start()
    {
        spawnableObjectsByLevelList = roomTemplate.enemiesByLevelList;
        randomSpawnableObject = new RandomSpawnableObject<EnemyDetailsSO>(spawnableObjectsByLevelList);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if(instEnemy != null)
            {
                Destroy(instEnemy);
            }

            EnemyDetailsSO enemyDetails = randomSpawnableObject.GetItem();

            if(enemyDetails != null )
            {
                instEnemy = Instantiate(enemyDetails.EnemyPrefab, HelperUtilities
                    .GetSpawnPositionNearestToPlayer(HelperUtilities
                    .GetMouseWorldPosition()), Quaternion.identity);
            }
        }
    }
}
