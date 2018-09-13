using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class AIMemory
{
    private class NodeCache
    {
        private int index = 0;
        private List<Node>[] nodeLists = new List<Node>[2];

        public NodeCache()
        {
            nodeLists[0] = new List<Node>();
            nodeLists[1] = new List<Node>();
        }

        public List<Node> GetListToPrepare()
        {
            return nodeLists[index % 2];
        }

        public List<Node> GetFinishedList()
        {
            return nodeLists[(index + 1) % 2];
        }

        public void SwapLists()
        {
            index = (index + 1) % 2;
        }
    }

    private MapToAreaPointer[,] mapCache;
    private Node actualAINode; //Refrenziert den Konten, auf welchem sich die AI befindet
    private Node targetNode;
    private LinkedList<PlatformArea> allAreas;
    private TransitionClaculator tCalc;
    private NodeCache nodeCache = new NodeCache();

    private IJumpRequest aiLocalSolver;
    private IEnvironmentAnalyser environmentAnalyser;

    private int rangeX;
    private int rangeY;

    private Vector2 aiPosition;
    int maxAreaWidth = 0;

    float boxFactor = 3.1f; //Breite eines Blocks im Spiel


    public int charachterHight = 2;
    public int jumpWidth = 6;
    public int jumpHight = 4;

    private int offSetX = 0;
    private int offSetY = 0;

    private Thread _calcThread;

    public AIMemory(int rangeX, int rangeY, Vector2 aiPosition, IEnvironmentAnalyser ea, IJumpRequest ailocal, float boxFactor)
    {
        mapCache = CreateMapCache(rangeX, rangeY);
        this.boxFactor = boxFactor;
        this.rangeX = rangeX;
        this.rangeY = rangeY;
        environmentAnalyser = ea;
        //AllNodesIntern = new List<Node>();
        allAreas = new LinkedList<PlatformArea>();

        this.aiPosition = aiPosition;
        offSetX = -1 * DivByBoxFactor(aiPosition.x) + rangeX;
        offSetY = -1 * DivByBoxFactor(aiPosition.y) + rangeY;

        aiLocalSolver = ailocal;
        tCalc = new TransitionClaculator(this, aiLocalSolver);

    }

    private MapToAreaPointer[,] CreateMapCache(int localRangeX, int localRangeY)
    {
        MapToAreaPointer[,] toReturn = new MapToAreaPointer[2 * localRangeY + 1, 2 * localRangeX + 1];
        for (int x = 0; x < toReturn.GetLength(1); x++)
        {
            for (int y = 0; y < toReturn.GetLength(0); y++)
            {
                toReturn[y, x] = new MapToAreaPointer();
            }
        }
        return toReturn;
    }

    private void UpdateMapCacheSize(int localRangeX, int localRangeY)
    {
        if (mapCache.GetLength(1) != 2 * localRangeX + 1 || mapCache.GetLength(0) != 2 * localRangeY + 1)
        {
            mapCache = CreateMapCache(localRangeX, localRangeY);
        }
    }

    /// <summary>
    /// Teilt den übergebenen Float durch die Breite eines Blockes.
    /// (Wird verwendet um Koordinaten in Array Koordinaten umzurechnen)
    /// </summary>
    /// <param name="a"></param>
    /// <returns>float/boxFactor</returns>
    public int DivByBoxFactor(float a)
    {
        return Mathf.RoundToInt(a / boxFactor);
    }

    /// <summary>
    /// Multipliziert einen int mit der Blockbreite.
    /// (Rückrechnung von Array Koordinaten in reale Koordinaten)
    /// </summary>
    /// <param name="a"></param>
    /// <returns>float * boxFactor</returns>
    public float MultWithBoxFactor(int a)
    {
        return RoundedMath.Mult(a, boxFactor);
    }

    public Vector2 NodeToArrayCoordinate(Node n)
    {
        return new Vector2(n.X + offSetX, n.Y + offSetY);
    }

    private List<Node> AllNodesIntern
    {
        get { return nodeCache.GetListToPrepare(); }
    }

    public List<Node> AllNodes
    {
        get { return nodeCache.GetFinishedList(); }
    }
    public LinkedList<PlatformArea> AllAreas
    {
        get { return allAreas; }
    }

    public Node ActualAiNode
    {
        get { return actualAINode; }
    }

    public Node TargetNode
    {
        get { return targetNode; }
    }

    public MapToAreaPointer[,] MapCache
    {
        get { return mapCache; }
    }

    public int MaxAreaWidth
    {
        get { return maxAreaWidth; }
        set { maxAreaWidth = value; }
    }

    public float BoxFactor { get { return boxFactor; } }

    public void AddArea(int x1, int x2, int y1)
    {
        allAreas.AddLast(new PlatformArea(x1, x2, y1, -1));
    }


    public bool UpdateMemory(Vector2 position, Node targetPosition)
    {
        return UpdateMemory(position, targetPosition, rangeX, rangeY);
    }


    private bool UpdateMemory(Vector2 position, Node targetPosition, int localRangeX, int localRangeY)
    {
        if (_calcThread == null)
        {
            List<Vector2> platfomrList = environmentAnalyser.CheckEnvironmet(position, localRangeX, localRangeY, boxFactor);
            _calcThread = new Thread(() => DoUpdateMemory(position, targetPosition, localRangeX, localRangeY, platfomrList));
            _calcThread.Start();
            return _calcThread.ThreadState != ThreadState.Running;
        }
        else
        {
            if (_calcThread.ThreadState == ThreadState.Stopped)
            {
                _calcThread.Join();
                _calcThread = null;
                return true;
            }
        }
        return false;
    }

    private void DoUpdateMemory(Vector2 position, Node targetPosition, int localRangeX, int localRangeY, List<Vector2> platformList)
    {
        UpdateMapCacheSize(localRangeX, localRangeY);
        aiPosition = position;
        offSetX = -1 * DivByBoxFactor(aiPosition.x) + localRangeX;
        offSetY = -1 * DivByBoxFactor(aiPosition.y) + localRangeY;
        nodeCache.SwapLists();
        AllNodesIntern.Clear();
        allAreas.Clear();
        targetNode = null;

        AnalyseEnvironment(localRangeX, localRangeY, platformList);

        if (targetPosition != null)
        {
            int tmpX = targetPosition.X + offSetX;
            int tmpY = targetPosition.Y + offSetY;
            if (tmpY - 1 > 0 && tmpY - 1 < mapCache.GetLength(0) && tmpX >= 0 && tmpX < mapCache.GetLength(1) && mapCache[tmpY - 1, tmpX].Type.Equals(SquareType.PLATFORM))
            {
                AllNodesIntern.Add(targetPosition);
                targetNode = targetPosition;
            }
            else if (tmpY - 2 > 0 && tmpY - 2 < mapCache.GetLength(0) && tmpX >= 0 && tmpX < mapCache.GetLength(1) && mapCache[tmpY - 2, tmpX].Type.Equals(SquareType.PLATFORM))
            {
                targetPosition.Y -= 1;
                AllNodesIntern.Add(targetPosition);
                targetNode = targetPosition;
            }
        }

        DetectAreas();
        CalculateTransitions();
        nodeCache.SwapLists();
    }

    public bool UpdateMemoryWithAdditionalRange(Vector2 position, Node targetPosition, int x, int y)
    {
        return UpdateMemory(position, targetPosition, rangeX + x, rangeY + y);
    }
    public void UpdateMemory(Vector2 position)
    {
        UpdateMemory(position, null, rangeX, rangeY);
    }

    private void AnalyseEnvironment(int localRangeX, int localRangeY, List<Vector2> platformList)
    {
        try
        {
            //List<Vector2> platformList = environmentAnalyser.CheckEnvironmet(aiPosition, localRangeX, localRangeY, boxFactor);

            foreach (MapToAreaPointer mtap in mapCache)
            {
                mtap.Type = SquareType.AIR;
                mtap.RangeToNextAbove = -1;
                mtap.Area = null;
            }

            foreach (Vector2 v in platformList)
            {
                int x = DivByBoxFactor(v.x) + offSetX;
                int y = DivByBoxFactor(v.y) + offSetY;
                if (x < 0 || y < 0 || x >= mapCache.GetLength(1) || y >= mapCache.GetLength(0))
                    continue;
                mapCache[y, x] = new MapToAreaPointer(SquareType.UNDEFINDED);

            }

            for (int x = 0; x < mapCache.GetLength(1); x++)
            {
                for (int y = 0; y < mapCache.GetLength(0); y++)
                {

                    if (mapCache[y, x].Type == SquareType.UNDEFINDED)
                    {

                        if (y < mapCache.GetLength(0) - charachterHight)
                        {
                            bool isPlatform = true;
                            for (int i = y + 1; i <= y + charachterHight; i++)
                            {
                                if (mapCache[i, x].Type != SquareType.AIR)
                                {
                                    isPlatform = false;
                                }
                            }
                            if (isPlatform)
                            {
                                mapCache[y, x].Type = SquareType.PLATFORM;
                            }
                            else
                            {
                                mapCache[y, x].Type = SquareType.BARRIER;
                            }

                        }
                    }
                }

            }
        }
        catch (System.IndexOutOfRangeException e)
        {
            //Debug.Log(e.StackTrace);
            Debug.LogError(e);
        }

    }


    private void DetectAreas()
    {
        for (int x = 0; x < mapCache.GetLength(1); x++)
        {
            for (int y = 0; y < mapCache.GetLength(0); y++)
            {
                if (mapCache[y, x].Type == SquareType.PLATFORM)
                {
                    for (int i = y + charachterHight; i < mapCache.GetLength(0); i++)
                    {
                        if (mapCache[i, x] != null && mapCache[i, x].Type != SquareType.AIR)
                        {
                            mapCache[y, x].RangeToNextAbove = i - y;
                            break;
                        }

                    }
                    if (x > 0)
                    {
                        if (mapCache[y, x - 1].Type == SquareType.PLATFORM && mapCache[y, x - 1].RangeToNextAbove == mapCache[y, x].RangeToNextAbove) //Prüfe, ob der Vorgenänger von der selben "ebene" überdeckt wird
                        {
                            if (maxAreaWidth <= 0 || mapCache[y, x - 1].Area.Width < maxAreaWidth)
                            {
                                mapCache[y, x].Area = mapCache[y, x - 1].Area; //Erweitere bei Wahr den Bereich des schon vorhandenen Knotens
                                mapCache[y, x].Area.XRight += 1;
                            }
                        }
                    }
                    if (mapCache[y, x].Area == null)
                    {
                        int x1 = x - offSetX;
                        int x2 = x - offSetX;
                        int y1 = y + 1 - offSetY;

                        AddArea(x1, x2, y1);
                        mapCache[y, x].Area = allAreas.Last.Value;
                    }
                }

            }
        }
        //Füge die Knoten ein, da nun alle Bereiche erkannt wurden.
        foreach (PlatformArea area in allAreas)
        {
            Node toAddLeft = new AStarNode(area.XLeft, area.YBottom);
            area.LeftNode = toAddLeft;
            AllNodesIntern.Add(toAddLeft);
            if (area.XLeft != area.XRight)
            {
                Node toAddRight = new AStarNode(area.XRight, area.YBottom);
                area.RightNode = toAddRight;
                AllNodesIntern.Add(toAddRight);
            }
        }

        //Füge einen Knoten ein, der die Aktuelle Position der AI darstellt, sofern ein solcher Knoten noch nicht vorhanden ist
        int xAi = DivByBoxFactor(aiPosition.x);
        int yAi = DivByBoxFactor(aiPosition.y) - 1;
        if (mapCache[yAi + offSetY, xAi + offSetX].Type != SquareType.AIR)
        {
            yAi += 1;
        }
        Node aiNode = new AStarNode(xAi, yAi);
        if (!AllNodesIntern.Contains(aiNode))
        {
            AllNodesIntern.Add(aiNode);
            actualAINode = aiNode;
        }
        else
        {
            actualAINode = AllNodesIntern[AllNodesIntern.IndexOf(aiNode)];
        }
    }

    private void CalculateTransitions()
    {

        for (int a = 0; a < AllNodesIntern.Count; a++)
        {
            for (int b = a + 1; b < AllNodesIntern.Count; b++)
            {
                Node nodeA = AllNodesIntern[a];
                Node nodeB = AllNodesIntern[b];

                Node left;
                Node right;
                if (nodeA.X < nodeB.X)
                {
                    left = nodeA;
                    right = nodeB;
                }
                else
                {
                    left = nodeB;
                    right = nodeA;
                }

                if (left.X == right.X)
                {
                    continue;
                }
                tCalc.CalculateTransition(nodeA, nodeB);
                TransitionType lTR = tCalc.GetLeftToRight();
                TransitionType rTL = tCalc.GetRightToLeft();
                if (!lTR.Equals(TransitionType.NONE))
                {
                    left.AddNeighbor(right, lTR);
                }
                if (!rTL.Equals(TransitionType.NONE))
                {
                    right.AddNeighbor(left, rTL);
                }
            }

        }
    }

    public void PrintMap()
    {
        string s = "";

        for (int y = mapCache.GetLength(0) - 1; y > -1; y--)
        {
            for (int x = 0; x < mapCache.GetLength(1); x++)
            {

                if (mapCache[y, x] != null)
                {
                    switch (mapCache[y, x].Type)
                    {
                        case SquareType.AIR: s += "O"; break;
                        case SquareType.BARRIER: s += "X"; break;
                        case SquareType.PLATFORM: s += "#"; break;
                        case SquareType.UNDEFINDED: s += "U"; break;
                    }


                }
                else s += "M";

            }
            s += "\n";
        }


        Debug.Log(s);
    }

}
