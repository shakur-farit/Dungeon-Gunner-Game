using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public Bounds roomColliderBounds;
    [HideInInspector] public int[,] aStarMovementPenalty;
    [HideInInspector] public int[,] aStarItemObstacles;
    [HideInInspector]public List<MoveItem> moveableItemsList;

    [Header("Object References")]
    [SerializeField] private GameObject _environmentGameObject;

    private BoxCollider2D _boxCollider;

    private void Awake()
    {
        _boxCollider = GetComponent <BoxCollider2D>();

        roomColliderBounds = _boxCollider.bounds;
    }

    private void Start()
    {

        UpdateMoveableObstacles();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == Settings.playerTag && room != GameManager.Instance.GetCurrentRoom)
        {
            room.isPreviouslyVisited = true;

            StaticEventHandler.CallRoomChangedEvent(room);
        }
    }

    public void Initialise(GameObject roomGameObject)
    {
        PopulateTilemapMemberVariables(roomGameObject);

        BlockOffUnusedDoorways();

        AddObstaclesAndPrefferedPath();

        CreateItemObstaclesArray();

        AddDoorsToRooms();

        DisableCollisionTilemapRender();
    }

    private void PopulateTilemapMemberVariables(GameObject roomGameObject)
    {
        grid = roomGameObject.GetComponentInChildren<Grid>();

        Tilemap[] tilemaps = roomGameObject.GetComponentsInChildren<Tilemap>();

        Dictionary<string, Tilemap> tilemapDictionary = new Dictionary<string, Tilemap>
    {
        {"groundTilemap", null},
        {"decoration1Tilemap", null},
        {"decoration2Tilemap", null},
        {"frontTilemap", null},
        {"collisionTilemap", null},
        {"minimapTilemap", null}
    };

        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemapDictionary.ContainsKey(tilemap.gameObject.tag))
            {
                tilemapDictionary[tilemap.gameObject.tag] = tilemap;
            }
        }

        groundTilemap = tilemapDictionary["groundTilemap"];
        decoration1Tilemap = tilemapDictionary["decoration1Tilemap"];
        decoration2Tilemap = tilemapDictionary["decoration2Tilemap"];
        frontTilemap = tilemapDictionary["frontTilemap"];
        collisionTilemap = tilemapDictionary["collisionTilemap"];
        minimapTilemap = tilemapDictionary["minimapTilemap"];
    }

    private void AddObstaclesAndPrefferedPath()
    {
        aStarMovementPenalty = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1,
            room.templateUpperBounds.y - room.templateLowerBounds.y + 1];

        for (int x = 0; x < (room.templateUpperBounds.x - room.templateLowerBounds.x + 1); x++)
        {
            for (int y = 0; y < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++)
            {
                aStarMovementPenalty[x, y] = Settings.defaultAStarMovementPenalty;

                TileBase tile = collisionTilemap.GetTile(new Vector3Int(
                    x + room.templateLowerBounds.x, y + room.templateLowerBounds.y, 0));

                foreach (TileBase collisionTile in GameResources.Instance.enemyUnwalkableCollisionTileArray)
                {
                    if(tile == collisionTile)
                    {
                        aStarMovementPenalty[x, y] = 0;
                        break;
                    }
                }

                if(tile == GameResources.Instance.preferredEnemyPathTile)
                {
                    aStarMovementPenalty[x, y] = Settings.prefferedPathAStarMovementPenalty;
                }
            }
        }
    }

    private void AddDoorsToRooms()
    {
        if (room.roomNodeType.isCorridorEW || room.roomNodeType.isCorridorNS)
            return;

        Dictionary<Orientation, Vector2> orientationOffsets = new Dictionary<Orientation, Vector2>
    {
        {Orientation.North, new Vector2(0.5f, 1f)},
        {Orientation.South, new Vector2(0.5f, 0f)},
        {Orientation.East, new Vector2(1f, 1.25f)},
        {Orientation.West, new Vector2(0f, 1.25f)}
    };

        foreach (Doorway doorway in room.doorwayList)
        {
            if (doorway.doorPrefab != null && doorway.isConnected)
            {
                float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;
                Vector2 offset = orientationOffsets[doorway.orientation];
                GameObject door = InstantiateDoor(doorway,
                    doorway.position.x + tileDistance * offset.x,
                    doorway.position.y + tileDistance * offset.y);

                Door doorComponent = door.GetComponent<Door>();

                if (room.roomNodeType.isBossRoom)
                {
                    doorComponent.isBossRoomDoor = true;

                    doorComponent.LockDoor();

                    GameObject skullIcon = Instantiate(GameResources.Instance.minimapSkullPrefab,
                        gameObject.transform);
                    skullIcon.transform.localPosition = door.transform.localPosition;
                }
            }
        }
    }

    private GameObject InstantiateDoor(Doorway doorway,
        float xPostion, float yPostion)
    {
        GameObject door = Instantiate(doorway.doorPrefab, gameObject.transform);
        door.transform.localPosition = new Vector3(xPostion, yPostion, 0f);
        return door;
    }
    
    private void BlockOffUnusedDoorways()
    {
        foreach (Doorway doorway in room.doorwayList)
        {
            if (doorway.isConnected)
                continue;

            if (collisionTilemap != null)
                BlockADoorwayOnTilemapLayer(collisionTilemap, doorway);

            if(minimapTilemap != null)
                BlockADoorwayOnTilemapLayer(minimapTilemap, doorway);

            if (groundTilemap != null)
                BlockADoorwayOnTilemapLayer(groundTilemap, doorway);

            if (decoration1Tilemap != null)
                BlockADoorwayOnTilemapLayer(decoration1Tilemap, doorway);

            if (decoration2Tilemap != null)
                BlockADoorwayOnTilemapLayer(decoration2Tilemap, doorway);

            if (frontTilemap != null)
                BlockADoorwayOnTilemapLayer(frontTilemap, doorway);
        }
    }

    private void BlockADoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway)
    {
        Dictionary<Orientation, Action<Tilemap, Doorway>> orientationToBlockMethod = 
            new Dictionary<Orientation, Action<Tilemap, Doorway>>
        {
            { Orientation.North, BlockDoorwayHorizontally },
            { Orientation.South, BlockDoorwayHorizontally },
            { Orientation.West, BlockDoorwayVertically },
            { Orientation.East, BlockDoorwayVertically },

        };

        if (orientationToBlockMethod.TryGetValue(doorway.orientation, out var blockMethod))
        {
            blockMethod(tilemap, doorway);
        }
    }

    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos,
                    startPosition.y - yPos, 0));

                tilemap.SetTile(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0),
                    tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0),
                    transformMatrix);
            }
        }
    }

    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos,
                   startPosition.y - yPos, 0));

                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0),
                    tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0),
                    transformMatrix);
            }
        }
    }

    private void DisableCollisionTilemapRender()
    {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }

    private void DisableRoomCollider()
    {
        _boxCollider.enabled = false;
    }

    private void EnableRoomCollider()
    {
        _boxCollider.enabled = true;
    }

    public void ActivateEnvironmentGameObject()
    {
        if(_environmentGameObject != null)
            _environmentGameObject.SetActive(true);
    }

    public void DeactivateEnvironmentGameObject()
    {
        if (_environmentGameObject != null)
            _environmentGameObject.SetActive(false);
    }

    public void LockDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray)
        {
            door.LockDoor();
        }

        DisableRoomCollider();
    }

    public void UnlockDoors(float doorUnlockDelay)
    {
        StartCoroutine(UnlockDoorsRoutin(doorUnlockDelay));
    }

    private IEnumerator UnlockDoorsRoutin(float doorUnlockDelay)
    {
        if (doorUnlockDelay > 0f)
            yield return new WaitForSeconds(doorUnlockDelay);

        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray)
        {
            door.UnlockDoor();
        }

        EnableRoomCollider();
    }

    private void CreateItemObstaclesArray()
    {
        aStarItemObstacles = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1,
            room.templateUpperBounds.y - room.templateLowerBounds.y + 1];
    }

    private void InitializeItemObstaclesArray()
    {
        for (int x = 0; x < (room.templateUpperBounds.x - room.templateLowerBounds.x + 1); x++)
        {
            for (int y = 0; y < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++)
            {
                aStarItemObstacles[x, y] = Settings.defaultAStarMovementPenalty;
            }
        }
    }

    public void UpdateMoveableObstacles()
    {
        InitializeItemObstaclesArray();

        foreach (MoveItem moveItem in moveableItemsList)
        {
            Vector3Int colliderBoundsMin = grid.WorldToCell(moveItem.ItemBoxCollieder.bounds.min);
            Vector3Int colliderBoundsMax = grid.WorldToCell(moveItem.ItemBoxCollieder.bounds.max);

            for (int i = colliderBoundsMin.x; i < colliderBoundsMax.x; i++)
            {
                for (int j = colliderBoundsMin.y; j < colliderBoundsMax.y; j++)
                {
                    aStarItemObstacles[i - room.templateLowerBounds.x, j - room.templateLowerBounds.y] = 0;
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(_environmentGameObject), _environmentGameObject);
    }
#endif
}
