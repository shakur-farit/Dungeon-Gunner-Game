using UnityEngine;

public class GridNodes
{
    private int _width;
    private int _height;

    private Node[,] _gridNode;

    public GridNodes(int width, int height)
    {
        _width = width;
        _height = height;

        _gridNode = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _gridNode[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    public Node GetGridNode(int xPosition, int yPosition)
    {
        if(xPosition < _width && yPosition < _height)
        {
            return _gridNode[xPosition, yPosition];
        }

        Debug.Log("Requested grid node is out of range");
        return null;
    }
}
