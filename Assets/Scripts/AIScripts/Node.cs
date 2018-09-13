using System.Collections;
using System.Collections.Generic;

public class Node
{
    LinkedList<Edge> edgesIn;
    LinkedList<Edge> edgesOut;
    private int x;
    private int y;

    PlatformArea area;

    public Node(Node n)
        : this(n.X, n.Y, n.EdgesIn, n.EdgesOut)
    {

    }

    public Node(int x, int y) : this(x, y, null, null) { }

    public Node(int x, int y, LinkedList<Edge> edgesIn, LinkedList<Edge> edgesOut)
    {
        this.x = x;
        this.y = y;
        if (edgesIn != null)
        {
            this.edgesIn = edgesIn;
        }
        else
        {
            this.edgesIn = new LinkedList<Edge>();
        }
        if (edgesOut != null)
        {
            this.edgesOut = edgesOut;
        }
        else
        {
            this.edgesOut = new LinkedList<Edge>();
        }
    }
    public void AddEdgeIn(Edge edge)
    {
        edgesIn.AddLast(edge);
    }
    public void AddEdgeOut(Edge edge)
    {
        edgesOut.AddLast(edge);
        edge.Target.AddEdgeIn(edge);
    }
    public int X { get { return x; } set { x = value; } }
    public int Y { get { return y; } set { y = value; } }

    public LinkedList<Edge> EdgesIn { get { return edgesIn; } set { edgesIn = value; } }
    public LinkedList<Edge> EdgesOut { get { return edgesOut; } set { edgesOut = value; } }


    public bool HasTransitionTo(Node target)
    {
        foreach (Edge e in edgesOut)
        {
            if (e.Target.Equals(target)) return true;
        }
        return false;
    }

    public void AddNeighbor(Node target, TransitionType t)
    {
        if (!HasTransitionTo(target))
            AddEdgeOut(new Edge(this, target, t));
    }

    public void ClearAll()
    {
        foreach (Edge e in edgesOut)
        {
            e.Target.edgesIn.Remove(e);
        }
        foreach (Edge e in edgesIn)
        {
            e.Source.edgesOut.Remove(e);
        }
        edgesIn.Clear();
        edgesOut.Clear();
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        Node other = obj as Node;
        if (other == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (x == other.X) && (y == other.Y);
    }

    public bool Equals(Node other)
    {
        if (other == null)
        {
            return false;
        }
        return (x == other.X && y == other.Y);
    }

    public Edge GetEdgeTo(Node target)
    {
        foreach (Edge e in edgesOut)
        {
            if (e.Target.Equals(target))
                return e;
        }
        return null;
    }

    public double Distance(Node target)
    {
        float x = X - target.X;
        float y = Y - target.Y;
        double tempDist = System.Math.Sqrt(System.Math.Pow(x, 2) + System.Math.Pow(y, 2));
        return tempDist;
    }
    public override string ToString()
    {
        return "(" + x + "|" + y + ")";
    }

    public override int GetHashCode()
    {
        var hashCode = 1198361892;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        return hashCode;
    }
}

public class AStarNode : Node
{
    public double g, h;
    private AStarNode predecessor;

    public AStarNode(int x, int y)
        : base(x, y)
    {
        g = 0;
    }
    public AStarNode(Node node)
        : base(node)
    {
    }
    public AStarNode Predecessor
    {
        get { return predecessor; }
        set { predecessor = value; }
    }

}
