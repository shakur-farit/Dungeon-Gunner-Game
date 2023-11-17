using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int GridPosition;
    public int GCost = 0; // distance from starting node
    public int HCost = 0; // distance from finishing node
    public Node ParentNode;

    public int FCost => GCost + HCost;

    public Node(Vector2Int gridPosition)
    {
        GridPosition = gridPosition;

        ParentNode = null;
    }

    public int CompareTo(Node nodeToCompare)
    {
        // compare will be < 0 if this instance Fcost is less than nodeToCompare.Fcost
        // compare will be > 0 if this instance Fcost is greater than nodeToCompare.Fcost
        // compare will be == 0 if the values are the same

        int compare = FCost.CompareTo(nodeToCompare.FCost);

        if (compare == 0)
            compare = HCost.CompareTo(nodeToCompare.HCost);

        return compare;
    }
}
