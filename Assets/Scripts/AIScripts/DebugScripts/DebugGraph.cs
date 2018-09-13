using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugGraph : MonoBehaviour
{


    public AIMemory aiMapGraph;
    public Shader shader;

    public Material lineMaterial;
    public AStarNode destination;
    public AStarNode source;
    private LinkedList<AStarNode> path;


    public LinkedList<AStarNode> Path
    {
        set
        {
            if (path == null) path = new LinkedList<AStarNode>();
            path.Clear();
            if (value != null && value.Count > 0)
            {
                foreach (AStarNode n in value)
                {
                    path.AddLast(n);
                }
            }
        }
        get
        {
            if (path == null) path = new LinkedList<AStarNode>();
            return path;
        }
    }
    void CreateLineMaterial()
    {
        if (!lineMaterial)
        {

            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    void OnPostRender()
    {

        CreateLineMaterial();


        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        if (aiMapGraph != null)
        {

            foreach (Node node in aiMapGraph.AllNodes)
            {
                Vector3 nodeV = new Vector3(RoundedMath.Mult(node.X, 3.1f), RoundedMath.Mult(node.Y, 3.1f));
                GL.Color(Color.black);
                GL.Vertex(nodeV + new Vector3(0, 1));
                GL.Vertex(nodeV + new Vector3(0, -1));
                if (node.EdgesOut != null)
                {
                    foreach (Edge e in node.EdgesOut)
                    {
                        Vector3 additional = new Vector3(0, 1f);
                        if (e.Source.X < e.Target.X)
                        {
                            additional *= -1;
                        }
                        Vector3 start2 = new Vector3(RoundedMath.Mult(e.Source.X, 3.1f), RoundedMath.Mult(e.Source.Y, 3.1f));
                        Vector3 end2 = new Vector3(RoundedMath.Mult(e.Target.X, 3.1f), RoundedMath.Mult(e.Target.Y, 3.1f));
                        if (e.TransType == TransitionType.JUMP)
                        {
                            GL.Color(Color.red);
                            GL.Vertex(start2 + additional);
                            GL.Vertex(end2 + additional);

                        }
                        else if (e.TransType == TransitionType.WALK)
                        {
                            GL.Color(Color.blue);
                            GL.Vertex(start2 + additional * 0.25f);
                            GL.Vertex(end2 + additional * 0.25f);
                        }
                        else if (e.TransType == TransitionType.FALL)
                        {
                            GL.Color(Color.yellow);
                            GL.Vertex(start2 + additional);
                            GL.Vertex(end2 + additional);
                        }
                        else
                        {
                            GL.Color(Color.black);
                            GL.Vertex(start2 + additional);
                            GL.Vertex(end2 + additional);
                        }

                    }
                }
            }
            foreach (PlatformArea area in aiMapGraph.AllAreas)
            {
                Vector3 start = new Vector3(RoundedMath.Mult(area.XLeft, 3.1f) - 0.55f, RoundedMath.Mult(area.YBottom, 3.1f) - 1.55f);
                Vector3 end = new Vector3(RoundedMath.Mult(area.XRight, 3.1f) + 0.55f, RoundedMath.Mult(area.YBottom, 3.1f) - 1.55f);
                GL.Color(Color.green);
                GL.Vertex(start);
                GL.Vertex(end);

            }
            if (Path.Count > 0)
            {
                GL.Color(Color.green);
                GL.Vertex(new Vector3(aiMapGraph.MultWithBoxFactor(source.X), aiMapGraph.MultWithBoxFactor(source.Y)));
                foreach (AStarNode n in Path)
                {
                    Vector3 point = new Vector3(aiMapGraph.MultWithBoxFactor(n.X), aiMapGraph.MultWithBoxFactor(n.Y));
                    GL.Vertex(point);
                    GL.Vertex(point);
                }
                GL.Vertex(new Vector3(aiMapGraph.MultWithBoxFactor(Path.Last.Value.X), aiMapGraph.MultWithBoxFactor(Path.Last.Value.Y)));
            }

        }

        GL.End();
    }



    public AIMemory AiMapGraph
    {
        set { aiMapGraph = value; }
    }
}
