using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    /// <summary>
    /// Builds a path for the room , from the startGridPosition
    /// to the endGridPosition, and adds movement steps to the
    /// returned Stack. Returns null if no path found.
    /// </summary>
    /// <param name="room"></param>
    /// <param name="startGridPosition"></param>
    /// <param name="endGridPosition"></param>
    /// <returns></returns>
    public static Stack<Vector3> BuildPath(Room room,
        Vector3Int startGridPosition, Vector3Int endGridPosition)
    {
        // Adjust position by lower bounds
        startGridPosition -= (Vector3Int)room.templateLowerBounds;
        endGridPosition -= (Vector3Int)room.templateLowerBounds;

        List<Node> openNodeList = new List<Node>();
        HashSet<Node> closeNodeHashSet = new HashSet<Node>();

        GridNodes gridNodes = new GridNodes(room.templateUpperBounds.x - room.templateLowerBounds.x + 1,
            room.templateUpperBounds.y - room.templateLowerBounds.y + 1);

        Node startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);
        Node targetNode = gridNodes.GetGridNode(endGridPosition.x, endGridPosition.y);

        Node targetPathNode = FindShortestPath(startNode, targetNode,
            gridNodes, openNodeList, closeNodeHashSet, room.instantiatedRoom);

        if (targetPathNode == null)
            return null;

        return CreatePathStack(targetPathNode, room);
    }

    private static Node FindShortestPath(Node startNode, Node targetNode, GridNodes gridNodes,
        List<Node> openNodeList, HashSet<Node> closeNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        openNodeList.Add(startNode);

        while (openNodeList.Count > 0)
        {
            openNodeList.Sort();

            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            if (currentNode == targetNode)
                return currentNode;

            closeNodeHashSet.Add(currentNode);

            EvaluteCurrentNodeNeighbours(currentNode, targetNode, gridNodes,
                openNodeList, closeNodeHashSet, instantiatedRoom);
        }

        return null;
    }

    private static void EvaluteCurrentNodeNeighbours(Node currentNode, Node targetNode,
        GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closeNodeHashSet,
        InstantiatedRoom instantiatedRoom)
    {
        Vector2Int currentNodeGridPosition = currentNode.GridPosition;

        Node validNeighbourNode;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j,
                    gridNodes, closeNodeHashSet, instantiatedRoom);

                if (validNeighbourNode == null)
                    continue;

                int newCostToNeighbour;

                newCostToNeighbour = currentNode.GCost + GetDistance(currentNode, validNeighbourNode);

                bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                if (newCostToNeighbour < validNeighbourNode.GCost || !isValidNeighbourNodeInOpenList)
                {
                    validNeighbourNode.GCost = newCostToNeighbour;
                    validNeighbourNode.HCost = GetDistance(validNeighbourNode, targetNode);
                    validNeighbourNode.ParentNode = currentNode;

                    if (!isValidNeighbourNodeInOpenList)
                        openNodeList.Add(validNeighbourNode);
                }
            }
        }
    }

    private static int GetDistance(Node currentNode, Node validNeighbourNode)
    {
        int distanceX = Mathf.Abs(currentNode.GridPosition.x - validNeighbourNode.GridPosition.x);
        int distanceY = Mathf.Abs(currentNode.GridPosition.y - validNeighbourNode.GridPosition.y);

        if (distanceX > distanceY)
            return 14 * distanceY + 10 * (distanceX - distanceY); // 10 used instead of 1,
        // and 14 is a pythagoras approximation SQRT(10*10 + 10*10) - to avoid using floats

        return 14 * distanceX + 10 * (distanceY - distanceX);
    }

    private static Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition, GridNodes gridNodes,
        HashSet<Node> closeNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        if (neighbourNodeXPosition >= instantiatedRoom.room.templateUpperBounds.x -
            instantiatedRoom.room.templateLowerBounds.x || neighbourNodeXPosition < 0 ||
            neighbourNodeYPosition >= instantiatedRoom.room.templateUpperBounds.y -
            instantiatedRoom.room.templateLowerBounds.y || neighbourNodeYPosition < 0)
        {
            return null;
        }

        Node neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

        if (closeNodeHashSet.Contains(neighbourNode))
        {
            return null;
        }

        return neighbourNode;
    }

    private static Stack<Vector3> CreatePathStack(Node targetPathNode, Room room)
    {
        Stack<Vector3> movementPathStack = new Stack<Vector3>();

        Node nextNode = targetPathNode;

        Vector3 cellMidPoint = room.instantiatedRoom.grid.cellSize * 0.5f;
        cellMidPoint.z = 0f;

        while(nextNode != null)
        {
            Vector3 worldPosition = room.instantiatedRoom.grid.CellToWorld(
                new Vector3Int(nextNode.GridPosition.x + room.templateLowerBounds.x,
                nextNode.GridPosition.y + room.templateLowerBounds.y, 0));

            worldPosition += cellMidPoint;

            movementPathStack.Push(worldPosition);

            nextNode = nextNode.ParentNode;
        }

        return movementPathStack;
    }
}
