using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehaviour<DungeonBuilder>
{
    public Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildSuccessful;

    private void OnEnable()
    {
        GameResources.Instance.dimmeMaterial.SetFloat("Alpha_Slider", 1f);
    }

    private void OnDisable()
    {
        GameResources.Instance.dimmeMaterial.SetFloat("Alpha_Slider", 1f);
    }

    protected override void Awake()
    {
        base.Awake();

        LoadRoomNodeTypeList();
    }

    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        roomTemplateList = currentDungeonLevel.roomTemplateList;

        LoadRoomTemplatesIntoDictioanry();

        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;

        while(!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.maxDungeonBuildAttempts)
        {
            dungeonBuildAttempts++;

            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);

            int dungeonRebuildAttemptsForNodeGraph = 0;
            dungeonBuildSuccessful = false;

            while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph)
            {
                ClearDungeon();

                dungeonRebuildAttemptsForNodeGraph++;

                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
            }

            if (dungeonBuildSuccessful)
                InstantiateRoomGameObjets();
        }

        return dungeonBuildSuccessful;
    }

    private void LoadRoomTemplatesIntoDictioanry()
    {
        roomTemplateDictionary.Clear();

        foreach (var roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
            {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
                continue;
            }
            
            Debug.Log("Dublicate Room Template Key In " + roomTemplateList);               
        }
    }

    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

        if (entranceNode == null)
        {
            Debug.Log("No Entrance Node");
            return false;
        }

        openRoomNodeQueue.Enqueue(entranceNode);

        bool noRoomOverlaps = true;

        noRoomOverlaps = ProccessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
            return true;
        
        return false;
    }

    private bool ProccessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, 
        Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
    {
        while(openRoomNodeQueue.Count> 0 && noRoomOverlaps == true)
        {
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            if (roomNode.roomNodeType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                room.isPositioned = true;

                dungeonBuilderRoomDictionary.Add(room.id, room);

                continue;
            }
           
            Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

            noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
           
        }

        return noRoomOverlaps;
    }

    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
    {
        bool roomOverlaps = true;

        while(roomOverlaps)
        {
            List<Doorway> unconnectedAvailableParentDoorways =
                GetUnconnectedAvailableDoorways(parentRoom.doorwayList).ToList();

            if (unconnectedAvailableParentDoorways.Count == 0)
                return false;

            Doorway doorwayParent =
                unconnectedAvailableParentDoorways[Random.Range(0, unconnectedAvailableParentDoorways.Count)];

            RoomTemplateSO roomTemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);

            Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

            if (PlaceTheRoom(parentRoom, doorwayParent, room))
            {
                roomOverlaps = false;

                room.isPositioned = true;

                dungeonBuilderRoomDictionary.Add(room.id, room);

                continue;
            }
            
            roomOverlaps = true;
            
        }

        return true;
    }

    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomTemplate = null;

        if (!roomNode.roomNodeType.isCorridor)
        {
            roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
            return roomTemplate;
        }

        Dictionary<Orientation, RoomTemplateSO> orientationToRoomTemplate = new Dictionary<Orientation, RoomTemplateSO>()
        {
            { Orientation.North, GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS)) },
            { Orientation.South, GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS)) },
            { Orientation.West, GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEW)) },
            { Orientation.East, GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEW)) },
            { Orientation.None, null }
        };

        if (orientationToRoomTemplate.ContainsKey(doorwayParent.orientation))
        {
            roomTemplate = orientationToRoomTemplate[doorwayParent.orientation];
        }

        return roomTemplate;
    }

    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)
    {
        Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorwayList);

        if(doorway == null)
        {
            doorwayParent.isUnavailable = true;

            return false;
        }

        Vector2Int parentDoorwayPosition =
            parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;

        Dictionary<Orientation, Vector2Int> orientationToAdjustment = new Dictionary<Orientation, Vector2Int>
        {
            { Orientation.North, new Vector2Int(0, -1) },
            { Orientation.East, new Vector2Int(-1, 0) },
            { Orientation.South, new Vector2Int(0, 1) },
            { Orientation.West, new Vector2Int(1, 0) },
        };

        Vector2Int adjustment = orientationToAdjustment.TryGetValue(doorway.orientation, out var value) ? value :
            Vector2Int.zero;

        room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        Room overlappingRoom = CheckForRoomOverlap(room);

        if(overlappingRoom == null)
        {
            doorwayParent.isConnected = true;
            doorwayParent.isUnavailable = true;

            doorway.isConnected = true;
            doorway.isUnavailable = true;

            return true;
        }

        doorwayParent.isUnavailable = true;
        return false;
    }

    private Doorway GetOppositeDoorway(Doorway doorwayParent, List<Doorway> doorwayList)
    {
        Dictionary<Orientation, Orientation> oppositeOrientation = new Dictionary<Orientation, Orientation>
        {
            { Orientation.East, Orientation.West },
            { Orientation.West, Orientation.East },
            { Orientation.North, Orientation.South },
            { Orientation.South, Orientation.North }
        };

        foreach (Doorway doorwayToCheck in doorwayList)
        {
            if (doorwayParent.orientation == oppositeOrientation[doorwayToCheck.orientation])
            {
                return doorwayToCheck;
            }
        }

        return null;
    }

    private Room CheckForRoomOverlap(Room roomToTest)
    {
        foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            if (room.id == roomToTest.id || !room.isPositioned)
                continue;

            if (IsOverLappingRoom(roomToTest, room))
                return room;
        }

        return null;
    }

    private bool IsOverLappingRoom(Room room1, Room room2)
    {
        bool isOverlappingX =
            IsOverLappingInterval(room1.lowerBounds.x, room1.upperBounds.x, 
            room2.lowerBounds.x, room2.upperBounds.x);
        bool isOverlappingY =
            IsOverLappingInterval(room1.lowerBounds.y, room1.upperBounds.y,
            room2.lowerBounds.y, room2.upperBounds.y);

        if(isOverlappingX && isOverlappingY)
            return true;

        return false;
    }

    private bool IsOverLappingInterval(int imin1, int imax1, int imin2, int imax2)
    {
        if (Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
            return true;

        return false;
    }

    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if(roomTemplate.roomNodeType == roomNodeType)
            {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        if (matchingRoomTemplateList.Count == 0)
            return null;

        return matchingRoomTemplateList[Random.Range(0, matchingRoomTemplateList.Count)];
    }

    private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorwayList)
    {
        foreach (Doorway doorway in roomDoorwayList)
        {
            if(!doorway.isConnected && !doorway.isUnavailable)
                yield return doorway;
        }
    }

    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        Room room = new Room();

        room.templateID = roomTemplate.guid;
        room.id = roomNode.id;
        room.prefab = roomTemplate.prefab;
        room.roomNodeType = roomTemplate.roomNodeType;
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.spawnPositionArray = roomTemplate.spawnPositionArray;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;
        room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList);
        room.doorwayList = CopyDoorwayList(roomTemplate.doorwayList);

        if (roomNode.parentRoomNodeIDList.Count == 0) // Entrance
        {
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;

            GameManager.Instance.SetCurrentRoom = room;

            return room;
        }
        
        room.parentRoomID = roomNode.parentRoomNodeIDList[0];
        
        return room;
    }

    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count > 0)
        {
            return roomNodeGraphList[Random.Range(0, roomNodeGraphList.Count)];
        }

        Debug.Log("No room node graph in list");
        return null;
    }

    private List<string> CopyStringList(List<string> oldStringList)
    {
        List<string> newStringList = new List<string>();

        foreach (string stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }

        return newStringList;
    }

    private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
    {
        List<Doorway> newDoorwayList = new List<Doorway>();

        foreach (Doorway doorway in oldDoorwayList)
        {
            Doorway newDoorway = new Doorway();

            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;

            newDoorwayList.Add(newDoorway);
        }

        return newDoorwayList;
    }

    private void InstantiateRoomGameObjets()
    {
        foreach (KeyValuePair<string,Room> keyValuePair in dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x,
                room.lowerBounds.y - room.templateLowerBounds.y, 0f);

            GameObject roomGameObject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            InstantiatedRoom instantiatedRoom = roomGameObject.GetComponentInChildren<InstantiatedRoom>();

            instantiatedRoom.room = room;

            instantiatedRoom.Initialise(roomGameObject);

            room.instantiatedRoom = instantiatedRoom;
        }
    }

    public RoomTemplateSO GetRoomTemplate(string roomTemplateID)
    {
        if(roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate))
            return roomTemplate;

        return null;
    }

    public Room GetRoomByRoomID(string roomID)
    {
        if(dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room))
            return room;

        return null;
    }

    private void ClearDungeon()
    {
        if(dungeonBuilderRoomDictionary.Count > 0)
        {
            foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
            {
                Room room = keyvaluepair.Value;

                if(room.instantiatedRoom != null)
                {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }

            dungeonBuilderRoomDictionary.Clear();
        }
    }
}
