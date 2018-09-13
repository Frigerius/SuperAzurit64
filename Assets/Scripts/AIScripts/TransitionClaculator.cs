using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransitionClaculator : ITransitionCalculator
{
    AIMemory aiMem;
    IJumpRequest aiLocalSolver;
    Node left;
    Node right;
    TransitionType leftToRight;
    TransitionType rightToLeft;
    Vector2 coordLeft;
    Vector2 coordRight;
    List<List<int[,]>> patterns;

    public TransitionClaculator(AIMemory aiMem, IJumpRequest aiLocalSolver)
    {
        this.aiMem = aiMem;
        this.aiLocalSolver = aiLocalSolver;
        leftToRight = TransitionType.NONE;
        rightToLeft = TransitionType.NONE;
        InitialisePatterns();
    }

    public void CalculateTransition(Node nodeA, Node nodeB)
    {
        leftToRight = TransitionType.NONE;
        rightToLeft = TransitionType.NONE;

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

        coordLeft = aiMem.NodeToArrayCoordinate(left);
        coordRight = aiMem.NodeToArrayCoordinate(right);

        if (CheckForWalk())
        {
            leftToRight = TransitionType.WALK;
            rightToLeft = TransitionType.WALK;
        }
        else
        {
            if (aiLocalSolver.CanYouJumpThis(NodeToVector(left), NodeToVector(right)))
            {
                if (CheckForJumpLeftToRight())
                {
                    leftToRight = TransitionType.JUMP;
                }
                if (CheckForJumpRightToLeft())
                {
                    rightToLeft = TransitionType.JUMP;
                }
            }

            if (CheckFallLeftToRight())
            {
                leftToRight = TransitionType.FALL;
            }
            else if (CheckJumpDownLeftToRight())
            {
                leftToRight = TransitionType.JUMP_DOWN;
            }

            if (CheckFallRightToLeft())
            {
                rightToLeft = TransitionType.FALL;
            }
            else if (CheckJumpDownRightToLeft())
            {
                rightToLeft = TransitionType.JUMP_DOWN;
            }
        }


    }

    public TransitionType GetLeftToRight()
    {
        return leftToRight;
    }
    public TransitionType GetRightToLeft()
    {
        return rightToLeft;
    }

    private bool CheckForWalk()
    {
        if (left.Y == right.Y)
        {
            if (IsSolidPlatform(coordLeft.y - 1, coordLeft.x, coordRight.x))
            {
                return NothingBetween(coordLeft.x, coordLeft.y, coordRight.x, coordRight.y + 1);
            }
        }
        return false;
    }


    private bool CheckForJumpLeftToRight()
    {

        //Sprung auf der selben Ebene über einen Abgrund.
        if (left.Y == right.Y)
        {
            if (CheckWithPatterns())
            {
                return true;
            }
            //Ist Sprung theoretisch möglich -> Zwischenraum frei.
            if (MapCache(coordLeft.y - 1, coordLeft.x).RangeToNextAbove >= 4 || MapCache(coordLeft.y - 1, coordLeft.x).RangeToNextAbove == -1)
            {
                if (NothingBetween(coordLeft.x + 1, coordLeft.y, coordRight.x - 1, coordRight.y + Mathf.Max(Mathf.Abs(right.X - left.X) - 1, 2)))
                {
                    if (NothingBetween(coordLeft.x + 1, coordLeft.y, coordRight.x - 1, coordRight.y + 5))
                    {
                        return true;
                    }
                    if (Mathf.Abs(right.X - left.X) < 7)
                    {
                        int addValue = (int)Mathf.Max(Mathf.Abs(right.X - left.X), 3);
                        int solideSpaceAbove = (Mathf.Min(2, Mathf.Abs(right.X - left.X) / 2));
                        //Teste, ob eine Decke vorhanden
                        //Links nach rechts
                        if (IsSolidPlatform(coordLeft.y + addValue, coordLeft.x, coordLeft.x + solideSpaceAbove))
                        {
                            return true;
                        }
                        int n = 1;
                        for (int i = addValue + 1; i <= Mathf.Min(addValue + 3, 6); i++)
                        {
                            if (NothingBetween(coordLeft.x, coordLeft.y + (i - 1), Mathf.Max(coordRight.x - n, coordLeft.x + solideSpaceAbove), coordRight.y + (i - 1)))
                            {
                                if (IsSolidPlatform(coordLeft.y + i, coordLeft.x, coordLeft.x + solideSpaceAbove))
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                return false;
                            }
                            n++;
                        }
                        return true;
                    }
                    else
                    {
                        return true;
                    }

                }
            }

            //Links niedriger als Rechts
        }
        else if (left.Y < right.Y)
        {
            if (Mathf.Abs(left.X - right.X) == 1)
            {
                if (NothingBetween(coordLeft.x, coordLeft.y, coordRight.x - 1, coordRight.y + 1)) // Prüfe ob genügend Platz für einen Sprung vorhanden
                {
                    return true;
                }
            }
            else if (Mathf.Abs(left.X - right.X) == 2)
            {
                if (MapCache(coordLeft.y - 1, coordLeft.x).RangeToNextAbove > 4 || MapCache(coordLeft.y - 1, coordLeft.x).RangeToNextAbove == -1)
                {
                    if (right.Y + 1 < left.Y - 1 + MapCache(coordLeft.y - 1, coordLeft.x).RangeToNextAbove || MapCache(coordLeft.y - 1, coordLeft.x).RangeToNextAbove == -1)
                    {
                        if (NothingBetween(coordLeft.x + 1, coordLeft.y, coordRight.x - 1, coordRight.y + 1)) // Prüfe ob genügend Platz für einen Sprung vorhanden
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                if (MapCache(coordLeft.y - 1, coordLeft.x).RangeToNextAbove > 4 || MapCache(coordLeft.y - 1, coordLeft.x).RangeToNextAbove == -1)
                {
                    int addValue = (int)Mathf.Max(Mathf.Abs(right.X - left.X), coordRight.y - coordLeft.y + 2);
                    int solideSpaceAbove = (Mathf.Min(2, Mathf.Abs(right.X - left.X) / 2));
                    if (NothingBetween(coordLeft.x + 1, coordLeft.y, coordRight.x - 1, coordLeft.y + addValue)) // Prüfe ob genügend Platz für einen Sprung vorhanden
                    {
                        if (Mathf.Abs(right.X - left.X) < 7)
                        {
                            //Teste, ob eine Decke vorhanden
                            //Links nach rechts
                            if (IsSolidPlatform(coordLeft.y + addValue + 1, coordLeft.x, coordLeft.x + solideSpaceAbove))
                            {
                                return true;
                            }
                            for (int i = addValue + 2; i <= Mathf.Min(addValue + 3, 6); i++)
                            {
                                if (NothingBetween(coordLeft.x, coordLeft.y + (i - 1), coordRight.x, coordRight.y + (i - 1)))
                                {
                                    if (IsSolidPlatform(coordLeft.y + i, coordLeft.x, coordLeft.x + solideSpaceAbove))
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    return false;
                                }

                            }
                            return true;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }

        }


        return false;
    }

    private MapToAreaPointer MapCache(float y, float x)
    {
        return aiMem.MapCache[(int)y, (int)x];
    }

    private bool CheckForJumpRightToLeft()
    {

        //Sprung auf der selben Ebene über einen Abgrund.
        if (left.Y == right.Y)
        {
            if (CheckWithPatterns())
            {
                return true;
            }
            //Ist Sprung theoretisch möglich -> Zwischenraum frei.
            if (MapCache(coordRight.y - 1, coordRight.x).RangeToNextAbove >= 4 || MapCache(coordRight.y - 1, coordRight.x).RangeToNextAbove == -1)
            {
                if (NothingBetween(coordLeft.x + 1, coordLeft.y, coordRight.x - 1, coordRight.y + Mathf.Max(Mathf.Abs(right.X - left.X) - 1, 2)))
                {
                    if (NothingBetween(coordLeft.x + 1, coordLeft.y, coordRight.x - 1, coordRight.y + 5))
                    {
                        return true;
                    }
                    if (Mathf.Abs(right.X - left.X) < 7)
                    {
                        int addValue = (int)Mathf.Max(Mathf.Abs(right.X - left.X), 3);
                        int solideSpaceAbove = (Mathf.Min(2, Mathf.Abs(right.X - left.X) / 2));
                        //Teste, ob eine Decke vorhanden
                        //Rechts nach Links
                        if (IsSolidPlatform(coordRight.y + addValue, coordRight.x - solideSpaceAbove, coordRight.x))
                        {
                            return true;
                        }
                        int n = 1;
                        for (int i = addValue + 1; i <= Mathf.Min(addValue + 3, 6); i++)
                        {
                            if (NothingBetween(Mathf.Max(coordLeft.x - n, coordRight.x - solideSpaceAbove), coordLeft.y + (i - 1), coordRight.x, coordRight.y + (i - 1)))
                            {
                                if (IsSolidPlatform(coordRight.y + i, coordRight.x - solideSpaceAbove, coordRight.x))
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                return false;
                            }
                            n++;
                        }
                        return true;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

        }
        else if (left.Y > right.Y)
        {

            if (Mathf.Abs(left.X - right.X) == 1)
            {
                if (NothingBetween(coordLeft.x + 1, coordRight.y, coordRight.x, coordLeft.y + 1)) // Prüfe ob genügend Platz für einen Sprung vorhanden
                {
                    return true;
                }
            }
            else if (Mathf.Abs(left.X - right.X) == 2)
            {
                if (MapCache(coordRight.y - 1, coordRight.x).RangeToNextAbove > 4 || MapCache(coordRight.y - 1, coordRight.x).RangeToNextAbove == -1)
                {
                    if (left.Y + 1 < right.Y - 1 + MapCache(coordRight.y - 1, coordRight.x).RangeToNextAbove || MapCache(coordRight.y - 1, coordRight.x).RangeToNextAbove == -1)
                    {
                        if (NothingBetween(coordLeft.x + 1, coordRight.y, coordRight.x - 1, coordLeft.y + 1)) // Prüfe ob genügend Platz für einen Sprung vorhanden
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                if (MapCache(coordRight.y - 1, coordRight.x).RangeToNextAbove > 4 || MapCache(coordRight.y - 1, coordRight.x).RangeToNextAbove == -1)
                {
                    int addValue = (int)Mathf.Max(Mathf.Abs(right.X - left.X), coordLeft.y - coordRight.y + 2);
                    int solideSpaceAbove = (Mathf.Min(2, Mathf.Abs(right.X - left.X) / 2));
                    if (NothingBetween(coordLeft.x + 1, coordRight.y, coordRight.x - 1, coordRight.y + addValue)) // Prüfe ob genügend Platz für einen Sprung vorhanden
                    {
                        if (Mathf.Abs(right.X - left.X) < 7)
                        {
                            //Teste, ob eine Decke vorhanden
                            //Rechts nach Links
                            if (IsSolidPlatform(coordRight.y + addValue + 1, coordRight.x - solideSpaceAbove, coordRight.x))
                            {
                                return true;
                            }
                            for (int i = addValue + 2; i <= Mathf.Min(addValue + 3, 6); i++)
                            {
                                if (NothingBetween(coordLeft.x, coordLeft.y + (i - 1), coordRight.x, coordRight.y + (i - 1)))
                                {
                                    if (IsSolidPlatform(coordRight.y + i, coordRight.x - solideSpaceAbove, coordRight.x))
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    return false;
                                }

                            }
                            return true;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }


        }

        return false;
    }

    private bool CheckFallLeftToRight()
    {
        if (left.Y <= right.Y)
        {
            return false;
        }

        if (Mathf.Abs(left.X - right.X) <= Mathf.Abs(left.Y - right.Y) || ((Mathf.Abs(left.X - right.X) <= 4 && Mathf.Abs(left.Y - right.Y) <= 3) && Mathf.Abs(left.X - right.X) - Mathf.Abs(left.Y - right.Y) <= 1))
        {
            if (NothingBetween(coordLeft.x + 1, coordRight.y, coordRight.x, coordLeft.y + 1))
            {
                return true;
            }
        }
        return false;
    }
    private bool CheckFallRightToLeft()
    {
        if (left.Y >= right.Y)
        {
            return false;
        }
        if (Mathf.Abs(left.X - right.X) <= Mathf.Abs(left.Y - right.Y) || ((Mathf.Abs(left.X - right.X) <= 4 && Mathf.Abs(left.Y - right.Y) <= 3) && Mathf.Abs(left.X - right.X) - Mathf.Abs(left.Y - right.Y) <= 1))
        {
            if (NothingBetween(coordLeft.x, coordLeft.y, coordRight.x - 1, coordRight.y + 1))
            {
                return true;
            }
        }
        return false;
    }
    private bool CheckJumpDownLeftToRight()
    {
        if (left.Y <= right.Y)
        {
            return false;
        }
        if (Mathf.Abs(left.X - right.X) > Mathf.Abs(left.Y - right.Y) && Mathf.Abs(left.X - right.X) <= Mathf.Abs(left.Y - right.Y) + 4) //Das ziel ist weiter Weg, als der Höhenunterschied -> es muss gesprungen werden
        {
            if (NothingBetween(coordLeft.x, coordLeft.y, coordLeft.x, coordLeft.y - 1 + 4) && NothingBetween(coordLeft.x + 1, coordRight.y, coordRight.x, coordLeft.y + 5))
            {
                return true;
            }
            if (CheckWithPatterns(coordLeft))
            {
                if (NothingBetween(coordLeft.x+1, coordRight.y, coordRight.x, coordLeft.y - 1))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool CheckJumpDownRightToLeft()
    {
        if (left.Y >= right.Y)
        {
            return false;
        }
        if (Mathf.Abs(left.X - right.X) > Mathf.Abs(left.Y - right.Y) && Mathf.Abs(left.X - right.X) <= Mathf.Abs(left.Y - right.Y) + 4) //Das ziel ist weiter Weg, als der Höhenunterschied -> es muss gesprungen werden
        {
            if (NothingBetween(coordRight.x, coordRight.y, coordRight.x, coordRight.y + 3) && NothingBetween(coordLeft.x, coordLeft.y, coordRight.x - 1, coordRight.y + 5))
            {
                return true;
            }
            if (CheckWithPatterns(new Vector2(coordLeft.x, coordRight.y)))
            {
                if (NothingBetween(coordLeft.x, coordLeft.y, coordRight.x-1, coordRight.y - 1))
                {
                    return true;
                }
            }
        }
        return false;
    }



    private bool NothingBetween(float x1, float y1, float x2, float y2)
    {
        return NothingBetween((int)x1, (int)y1, (int)x2, (int)y2);
    }

    private bool NothingBetween(int x1, int y1, int x2, int y2)
    {
        try
        {
            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    if (aiMem.MapCache[y, x].Type != SquareType.AIR) return false;
                }
            }
            return true;
        }
        catch (System.IndexOutOfRangeException)
        {
            return false;
        }
    }

    private bool IsSolidPlatform(float y, float x1, float x2)
    {
        return IsSolidPlatform((int)y, (int)x1, (int)x2);
    }
    private bool IsSolidPlatform(int y, int x1, int x2)
    {
        try
        {
            for (int x = x1; x <= x2; x++)
            {
                if (aiMem.MapCache[y, x].Type == SquareType.AIR) return false;
            }
            return true;
        }
        catch (System.IndexOutOfRangeException)
        {
            return false;
        }
    }

    private Vector2 NodeToVector(Node n)
    {
        return new Vector2(n.X, n.Y);
    }


    private bool CheckWithPatterns(Vector2 zeroSpot)
    {

        int index = Mathf.Abs(right.X - left.X) - 2;
        if(index >= patterns.Count)
        {
            return false;
        }
        try
        {
            foreach (int[,] pattern in patterns[index])
            {

                for (int x = 0; x < pattern.GetLength(1); x++)
                {
                    for (int y = 0; y < pattern.GetLength(0); y++)
                    {
                        if (pattern[y, x] == 1)
                        {
                            if (aiMem.MapCache[y + (int)zeroSpot.y, x + (int)zeroSpot.x].Type != SquareType.AIR) return false;
                        }
                    }
                }
                return true;
            }

        }
        catch (System.IndexOutOfRangeException)
        {

        }


        return false;
    }

    private bool CheckWithPatterns()
    {
        if (right.Y == left.Y)
        {
            return CheckWithPatterns(coordLeft);
        }

        return false;
    }

    void PrintPattern(int[,] pattern)
    {
        string s = "";
        for (int y = 0; y < pattern.GetLength(0); y++)
        {
            for (int x = 0; x < pattern.GetLength(1); x++)
            {

                s += "" + pattern[y, x];
            }
            s += "\n";
        }
        Debug.Log(s);
    }

    //Patterns

    void InitialisePatterns()
    {
        patterns = new List<List<int[,]>>();
        int[,] sixSpace = {
                          {0,0,1,1,1,1,0,0},
                          {0,1,1,1,1,1,1,0},
                          {1,1,1,1,1,1,1,1},
                          {1,1,1,1,1,1,1,1},
                          {1,1,1,1,1,1,1,1},
                          {1,1,1,1,1,1,1,1}
                      };
        int[,] fiveSpace = {
                          {0,1,1,1,1,1,0},
                          {0,1,1,1,1,1,0},
                          {1,1,1,1,1,1,1},
                          {1,1,1,1,1,1,1},
                          {1,1,1,1,1,1,1}
                      };
        int[,] fourSpace = {
                           {0,1,1,1,1,0},
                           {0,1,1,1,1,0},
                           {1,1,1,1,1,1},
                           {1,1,1,1,1,1},
                           {1,1,1,1,1,1}
                       };
        int[,] threeSpace = {
                            {0,1,1,1,0},
                            {1,1,1,1,1},
                            {1,1,1,1,1},
                            {1,1,1,1,1},
                        };
        int[,] twoSpace1 = {
                          {0,1,1,0},
                          {1,1,1,1},
                          {1,1,1,1},
                          {1,1,1,1},
                      };
        int[,] twoSpace2 = {
                          {0,0,0,0},
                          {1,1,1,1},
                          {1,1,1,1},
                          {1,1,1,1},
                      };
        int[,] oneSpace = {
                          {1,1,1},
                          {1,1,1},
                          {1,1,1},
                      };

        patterns.Add(new List<int[,]>());
        patterns[0].Add(oneSpace);
        patterns.Add(new List<int[,]>());
        patterns[1].Add(twoSpace1);
        patterns[1].Add(twoSpace2);
        patterns.Add(new List<int[,]>());
        patterns[2].Add(threeSpace);
        patterns.Add(new List<int[,]>());
        patterns[3].Add(fourSpace);
        patterns.Add(new List<int[,]>());
        patterns[4].Add(fiveSpace);
        patterns.Add(new List<int[,]>());
        patterns[5].Add(sixSpace);
        foreach (List<int[,]> l in patterns)
        {
            for (int i = 0; i < l.Count; i++)
            {
                l[i] = InvertArray(l[i]);
            }
        }

    }

    int[,] InvertArray(int[,] a)
    {
        int[,] toReturn = new int[a.GetLength(0), a.GetLength(1)];
        for (int y = 0; y < a.GetLength(0); y++)
        {
            for (int x = 0; x < a.GetLength(1); x++)
            {
                toReturn[y, x] = a[a.GetLength(0) - 1 - y, x];
            }
        }
        return toReturn;
    }
}
