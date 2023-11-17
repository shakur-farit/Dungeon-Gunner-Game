using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{
    private InstantiatedRoom _instantiatedRoom;
    private Grid _grid;
    private Tilemap _frontTilemap;
    private Tilemap _pathTilemap;
    private Vector3Int _startGridPosition;
    private Vector3Int _endGridPosition;
    private TileBase _startPathTile;
    private TileBase _finishPathTile;

    private Vector3Int _noValue = new Vector3Int(9999, 9999, 9999);
    private Stack<Vector3> _pathStack;

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void Start()
    {
        _startPathTile = GameResources.Instance.preferredEnemyPathTile;
        _finishPathTile = GameResources.Instance.enemyUnwalkableCollisionTileArray[0];
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs obj)
    {
        _pathStack = null;
        _instantiatedRoom = obj.Room.instantiatedRoom;
        _frontTilemap = _instantiatedRoom.transform.Find("Grid/Tilemap4_Front").GetComponent<Tilemap>();
        _grid = _instantiatedRoom.transform.GetComponentInChildren<Grid>();
        _startGridPosition = _noValue;
        _endGridPosition = _noValue;

        SetUpPathTilemap();
    }

    private void SetUpPathTilemap()
    {
        Transform tilemapCloneTransform = _instantiatedRoom.transform.Find("Grid/Tilemap4_Front(Clone)");

        if(tilemapCloneTransform == null)
        {
            _pathTilemap = Instantiate(_frontTilemap, _grid.transform);
            _pathTilemap.GetComponent<TilemapRenderer>().sortingOrder = 2;
            _pathTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
            _pathTilemap.gameObject.tag = "Untagged";
        }
        else
        {
            _pathTilemap = _instantiatedRoom.transform.Find("Grid/Tilemap4_Front(Clone)").GetComponent<Tilemap>();
            _pathTilemap.ClearAllTiles();
        }
    }

    private void Update()
    {
        if (_instantiatedRoom == null || _startPathTile == null || _finishPathTile == null || _grid == null || _pathTilemap == null)
            return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            ClearPath();
            SetStartPosition();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ClearPath();
            SetEndPosition();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            DisplayPath();
        }
    }

    private void ClearPath()
    {
        if (_pathStack == null)
            return;

        foreach (Vector3 worldPostion in _pathStack)
        {
            _pathTilemap.SetTile(_grid.WorldToCell(worldPostion), null);
        }
        _pathStack = null;

        _endGridPosition = _noValue;
        _startGridPosition = _noValue;
    }

    private void SetStartPosition()
    {
        if(_startGridPosition == _noValue)
        {
            _startGridPosition = _grid.WorldToCell(HelperUtilities.GetMouseWorldPosition());

            if (!IsPositionWithinBounds(_startGridPosition))
            {
                _startGridPosition = _noValue;
                return;
            }

            _pathTilemap.SetTile(_startGridPosition, _startPathTile);
        }
        else
        {
            _pathTilemap.SetTile(_startGridPosition, null);
            _startGridPosition = _noValue;
        }
    }

    private void SetEndPosition()
    {
        if(_endGridPosition == _noValue)
        {
            _endGridPosition = _grid.WorldToCell(HelperUtilities.GetMouseWorldPosition());

            if (!IsPositionWithinBounds(_endGridPosition))
            {
                _endGridPosition = _noValue;
                return;
            }

            _pathTilemap.SetTile(_endGridPosition, _finishPathTile);
        }
        else
        {
            _pathTilemap.SetTile(_endGridPosition, null);
            _endGridPosition = _noValue;
        }
    }

    private bool IsPositionWithinBounds(Vector3Int startGridPosition)
    {
        if(startGridPosition.x < _instantiatedRoom.room.templateLowerBounds.x||startGridPosition.x > _instantiatedRoom.room.templateUpperBounds.x ||
            startGridPosition.y < _instantiatedRoom.room.templateLowerBounds.y || startGridPosition.y > _instantiatedRoom.room.templateUpperBounds.y)
        {
            return false;
        }

        return true;
    }

    private void DisplayPath()
    {
        if (_startGridPosition == _noValue || _endGridPosition == _noValue)
            return;

        _pathStack = AStar.BuildPath(_instantiatedRoom.room, _startGridPosition, _endGridPosition);

        if (_pathStack == null)
            return;

        foreach(Vector3 worldPosition in _pathStack)
        {
            _pathTilemap.SetTile(_grid.WorldToCell(worldPosition), _startPathTile);
        }
    }
}
