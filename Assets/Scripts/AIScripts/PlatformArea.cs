using UnityEngine;
using System.Collections;

public class PlatformArea
{

    int xLeft;
    int xRight;
    int yTop;
    int yBottom;
    Node leftNode;
    Node rightNode;



    public PlatformArea(int x1, int x2, int y1, int y2, bool initializeNodes)
    {
        xLeft = x1;
        xRight = x2;
        yBottom = y1;
        yTop = y2;
        if (initializeNodes)
        {
            leftNode = new Node(x1, y1, null, null);
            rightNode = new Node(x2, y1, null, null);
            Reconnect();
        }
    }

    public PlatformArea(int x1, int x2, int y1, int y2)
        : this(x1, x2, y1, y2, false)
    {

    }

    public int XLeft
    {
        get { return xLeft; }
        set
        {
            xLeft = value;
            if (leftNode != null)
            {
                leftNode.X = value;
                leftNode.ClearAll();
                Reconnect();
            }

        }
    }
    public int XRight
    {
        get { return xRight; }
        set
        {
            xRight = value;
            if (rightNode != null)
            {
                rightNode.X = value;
                rightNode.ClearAll();
                Reconnect();
            }
        }
    }
    public int YTop { get { return yTop; } set { yTop = value; } }
    public int YBottom { get { return yBottom; } set { yBottom = value; } }

    public Node LeftNode
    {
        get { return leftNode; }
        set
        {
            if (leftNode != null)
            {
                leftNode.ClearAll();
            }
            leftNode = value;
            Reconnect();
        }
    }

    public Node RightNode
    {
        get { return rightNode; }
        set
        {
            if (rightNode != null)
            {
                rightNode.ClearAll();
            }
            rightNode = value;
            Reconnect();
        }
    }

    private void Reconnect()
    {
        if (leftNode != null && rightNode != null)
        {
            leftNode.AddNeighbor(rightNode, TransitionType.WALK);
            rightNode.AddNeighbor(leftNode, TransitionType.WALK);
        }
    }
    public int Width
    {
        get { return xRight - xLeft + 1; }
    }
}
