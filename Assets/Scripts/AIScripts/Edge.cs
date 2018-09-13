using System.Collections;

public class Edge
{
    private Node source;
    private Node target;
    private TransitionType transType;

    public Edge(Node source, Node target, TransitionType transitionType)
    {

        this.source = source;
        this.target = target;
        this.transType = transitionType;
    }

    public Node Source
    {
        get { return source; }
        set { source = value; }
    }
    public Node Target
    {
        get { return target; }
        set { target = value; }
    }

    public TransitionType TransType
    {
        get { return transType; }
        set { transType = value; }
    }

    public override string ToString()
    {
        return source.ToString() + " -> " + target.ToString();
    }
}
