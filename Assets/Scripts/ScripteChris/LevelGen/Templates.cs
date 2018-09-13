using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(BlockGenerator))]
[RequireComponent(typeof(MapGeneratorCA))]

public class Templates : MonoBehaviour {

    [Range(1, 2)]
    public int levelType = 1;

    [Range(1,10)]
    public int GridWidth = 5;

    [Range(1,10)]
    public int GridHeight = 5;

    [Range(0, 100)]
    public int trapPercentage = 100;

    public BlockGenerator blockGen;
    public MapGeneratorCA cellularScript;
    public int smoothAttempts = 0;
    List<LevelTemplate> templateList = new List<LevelTemplate>();
    List<Vector3> spawnPoints = new List<Vector3>();
    List<LevelTemplate> leftList = new List<LevelTemplate>();
    List<LevelTemplate> rightList = new List<LevelTemplate>();
    List<LevelTemplate> upList = new List<LevelTemplate>();
    List<LevelTemplate> downList = new List<LevelTemplate>();
    List<LevelTemplate> rightAndLeftList = new List<LevelTemplate>();

	// Use this for initialization
	void Start () {

        

       


	}

    public void Initialize()
    {
        blockGen = GetComponent<BlockGenerator>();
        cellularScript = GetComponent<MapGeneratorCA>();
    }

    public void ClearTemplateList()
    {
        
        templateList.Clear();
        
    }

    public void TestLevel(bool spawnTraps = true)
    {
        if (blockGen == null || cellularScript == null)
        {
            Initialize();
        }
        if (templateList.Count <= 0)
        {
            
            AddTemplates();
            RotateTemplates();
        }
        int[,] test;
        if (levelType == 1)
        {
            test = TemplateRandomizer(15, 1);
        }
        else if (levelType == 2)
        {
            test = GenerateGridLevel(GridWidth, GridHeight);
        }
        else
        {
            test = TemplateRandomizer(15, 1);
        }
        
        for (int i = 0; i < smoothAttempts; i++)
        {
            cellularScript.SmoothOnce(test);
        }
        blockGen.SpawnLevel(test, spawnTraps);
        
    }
    
	
	// Update is called once per frame
	void Update () {
	
	}

    void CalculateLists()
    {
        if (leftList.Count == 0 && rightList.Count == 0 && upList.Count == 0 && downList.Count == 0)
        {
            foreach (LevelTemplate temp in templateList)
            {
                if (temp.HasLeftWay())
                {
                    leftList.Add(temp);
                }
                if (temp.HasRightWay())
                {
                    rightList.Add(temp);
                }
                if (temp.HasUpWay())
                {
                    upList.Add(temp);
                }
                if (temp.HasDownWay())
                {
                    downList.Add(temp);
                }
                if (temp.HasLeftWay() && temp.HasRightWay())
                {
                    rightAndLeftList.Add(temp);
                }
            }
        }
    }

    public int[,] GenerateGridLevel(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            return new int[0, 0];
        }
        int[,] a = new int[width * 20, height * 20];
        

        CalculateLists();
        LevelTemplate last = rightList[Random.Range(0, rightList.Count)];
        LevelTemplate current;
        for (int y = 0; y < height; y++ )
        {
            for (int x = 0; x < width; x++)
            {
                if (y % 2 == 0)
                {
                    if (x == 0)
                    {
                        do
                        {
                            current = rightList[Random.Range(0, rightList.Count)];
                        } while (!current.HasDownWay());
                        last = current;
                        if (y == height - 1)
                        {
                            PlaceInto(a, last.levelPart, x * 20, y * 20, 12, true, false, false, true);
                        }
                        else if (y == 0)
                        {

                            PlaceInto(a, last.levelPart, x * 20, y * 20, 10, true, false, true, false);
                        }
                        else
                        {
                            PlaceInto(a, last.levelPart, x * 20, y * 20, 12, true, false, false, false);
                        }


                    }
                    else if (x == width - 1)
                    {
                        do
                        {
                            current = rightAndLeftList[Random.Range(0, rightAndLeftList.Count)];
                        } while (!last.CanCrossRightTo(current) || !current.HasUpWay());
                        last = current;
                        if (y == height - 1)
                        {
                            PlaceInto(a, last.levelPart, x * 20, y * 20, 12, false, true, false, true);
                        }
                        else if (y == 0)
                        {
                            PlaceInto(a, last.levelPart, x * 20, y * 20, 12, false, true, true, false);
                        }
                        else
                        {
                            PlaceInto(a, last.levelPart, x * 20, y * 20, 12, false, true, false, false);
                        }


                    }
                    else
                    {
                        do
                        {
                            current = rightAndLeftList[Random.Range(0, rightAndLeftList.Count)];
                        } while (!last.CanCrossRightTo(current));
                        last = current;
                        if (y == height - 1)
                        {
                            PlaceInto(a, last.levelPart, x * 20, y * 20, 12, false, false, false, true);
                        }
                        else if (y == 0)
                        {
                            PlaceInto(a, last.levelPart, x * 20, y * 20, 12, false, false, true, false);
                        }
                        else
                        {
                            PlaceInto(a, last.levelPart, x * 20, y * 20, 12, false, false, false, false);
                        }
                    }
                }
                else if (y % 2 == 1)
                {
                    if (x == 0)
                    {
                        do
                        {
                            current = rightList[Random.Range(0, rightAndLeftList.Count)];
                        } while (!current.HasUpWay());
                        last = current;
                        if (y == height - 1)
                        {
                            PlaceInto(a, last.levelPart, x * 20, y * 20, 12, true, false, false, true);
                        }
                        else
                        {
                            PlaceInto(a, last.levelPart, x * 20, y * 20, 12, true, false, false, false);
                        }

                    }
                    else if (x == width - 1)
                    {
                        do
                        {
                            current = leftList[Random.Range(0, leftList.Count)];
                        } while (!last.CanCrossRightTo(current) || !current.HasDownWay());
                        last = current;
                        if (y == height - 1)
                        {
                            PlaceInto(a, last.levelPart, x * 20, y * 20, 12, false, true, false, true);
                        }
                        else
                        {
                            PlaceInto(a, last.levelPart, x * 20, y * 20, 12, false, true, false, false);
                        }
                    }
                    else
                    {
                        do
                        {
                            current = rightAndLeftList[Random.Range(0, rightAndLeftList.Count)];
                        } while (!last.CanCrossRightTo(current));
                        last = current;
                        if (y == height - 1)
                        {
                            PlaceInto(a, last.levelPart, x * 20, y * 20, 12, false, false, false, true);
                        }
                        else
                        {
                            PlaceInto(a, last.levelPart, x * 20, y * 20, 12, false, false, false, false);
                        }
                    }
                }
                if (y == height - 1 && x == width - 1)
                {
                    PlaceInto(a, last.levelPart, x * 20, y * 20, 11, false, true, false, true);
                }
            }
        }




            return a;
    }

    public int[,] TemplateRandomizer(int widthCount, int layers)
    {
        if (widthCount <= 0 || layers <= 0)
        {
            return new int[0, 0];
        }
        int[,] a = new int[widthCount*20, layers*20];
        int currentX = 0;
        int currentY = 0;

        LevelTemplate last;
        LevelTemplate current;
        do
        {
            last = templateList[Random.Range(0, templateList.Count)];

        } while (!last.HasRightWay());

        
        
        PlaceInto(a, last.levelPart, currentX * 20, currentY * 20, 10, true, false, true, true);
        currentX++;
        while (currentY < layers)
        {
            while (currentX < widthCount)
            {

                do
                {
                    current = templateList[Random.Range(0, templateList.Count)];
                    

                } while (!last.CanCrossRightTo(current));
                
                last = current;
                if (currentX + 1 >= widthCount)
                {
                    PlaceInto(a, current.levelPart, currentX * 20, currentY * 20, 11, false, true, true, true);
                }
                else
                {
                    PlaceInto(a, current.levelPart, currentX*20, currentY*20, 12, false, false, true, true);
                }
                currentX++;
            }
            currentY++;
            currentX = 0;
        }
        


        return a;
    }

    private void RotateTemplates()
    {
        /*
         
        List<int[,]> tempList = new List<int[,]>();
        foreach (LevelTemplate tmplt in templateList)
        {
            tempList.Add(blockGen.RotateArrayClockwise(tmplt.levelPart));
        }
        List<LevelTemplate> templateTempList = new List<LevelTemplate>();
        foreach (int[,] a in tempList)
        {
            templateTempList.Add(new LevelTemplate(a));
        }
        templateList = templateTempList;
        */
        List<LevelTemplate> tempList = new List<LevelTemplate>();
        for (int i = 0; i < templateList.Count; i++)
        {
            int[,] a = blockGen.RotateArrayClockwise(templateList[i].levelPart);

            tempList.Add(new LevelTemplate(a, templateList[i]));
        }
        templateList = tempList;
    }

    public void PlaceInto(int[,] level, int[,] part, int x, int y, int spawnChoice = 12, bool closeLeft = false, bool closeRight = false, bool closeDown = false, bool closeUp = false)
    {
        for (int i = 0; i <= part.GetUpperBound(0) && i + x <= level.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= part.GetUpperBound(1) && j + y <= level.GetUpperBound(1); j++)
            {
                int blockType = part[i,j];
                if (blockType == 2 || blockType == 12)
                {
                    level[i + x, j + y] = spawnChoice;
                }
                else if (blockType == 5 || blockType == 6 || blockType == 4)
                {
                    if(Random.Range(0,100) < trapPercentage){
                        level[i + x, j + y] = blockType;
                    }
                    else
                    {
                        level[i + x, j + y] = 0;
                    }


                }else
                {
                    if ((closeLeft && i == 0) || (closeUp && j == part.GetUpperBound(1)) || (closeDown && j == 0) || (closeRight && i == part.GetUpperBound(0)))
                    {
                        level[i + x, j + y] = 1;
                    }
                    else
                    {
                        level[i + x, j + y] = blockType;
                    }
                }
            }
        }

    }

    

    public List<Vector3> GetAISpawns()
    {
        return spawnPoints;
    }

    private void AddTemplates()
    {
        /* Leeres Template zum kopieren
        templateList.Add(new LevelTemplate(new int[,] {    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},        
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1}, 
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1}, 
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}, false, false, false, false, false, false));
        */
        templateList.Add(new LevelTemplate(new int[,] {    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},  
                                                           { 0, 4, 4, 0, 4, 4, 0, 0, 0, 4, 0, 0, 0, 0, 4, 4, 0, 0, 0, 0},  
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1},  
                                                           { 1, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 3, 3, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1},  
                                                           { 1, 1, 1, 1, 1, 0, 0, 4, 0, 0, 4, 0, 0, 4, 0, 0, 0, 6, 1, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 6, 1, 1, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 1, 1, 1, 1},        
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1}, 
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1}, 
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1},
                                                           { 1, 0, 0, 0, 0, 6, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 0, 0, 0, 6, 1, 1, 3, 3, 3, 3, 1, 1, 0, 0, 3, 3, 0, 0, 1},
                                                           { 0, 0, 0, 0, 1, 1, 3, 3, 3, 4, 3, 3, 1, 0, 3, 0, 0, 3, 0, 0},
                                                           { 0, 0, 0, 6, 1, 3, 3, 3, 7, 1, 3, 3, 1, 1, 3, 0, 0, 3, 0, 0},
                                                           { 0, 2, 0, 1, 1, 3, 3, 3, 1, 1, 4, 3, 0, 1, 1, 5, 5, 3, 0, 0},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}, true, true, true, true, false, false));

        templateList.Add(new LevelTemplate(new int[,] {    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                                           { 0, 0, 0, 0, 0, 3, 0, 0, 3, 0, 0, 3, 3, 0, 0, 0, 0, 0, 0, 1},
                                                           { 0, 0, 0, 0, 0, 3, 0, 0, 3, 0, 0, 3, 3, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 4, 4, 0, 4, 4, 0, 4, 4, 3, 3, 4, 4, 0, 0, 0, 0, 1},  
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1},  
                                                           { 1, 0, 0, 0, 0, 4, 0, 4, 0, 4, 2, 4, 0, 4, 0, 0, 0, 0, 1, 1},  
                                                           { 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},        
                                                           { 1, 1, 0, 0, 0, 0, 0, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1}, 
                                                           { 1, 1, 0, 0, 0, 0, 4, 3, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1}, 
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1},
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 0, 3, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1},
                                                           { 1, 3, 3, 3, 3, 3, 0, 0, 3, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                                           { 1, 0, 3, 3, 3, 1, 0, 3, 0, 3, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0},
                                                           { 1, 0, 7, 3, 4, 1, 3, 6, 6, 0, 6, 6, 3, 0, 0, 0, 1, 1, 0, 0},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}, true, false, false, true, false, false));

        templateList.Add(new LevelTemplate(new int[,] {    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},  
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 4, 4, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0},  
                                                           { 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 1},  
                                                           { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 1, 1, 6, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 3, 0, 1},  
                                                           { 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 3, 3, 1},  
                                                           { 1, 0, 0, 0, 1, 6, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 3, 1, 1},  
                                                           { 1, 0, 0, 0, 1, 1, 6, 0, 0, 0, 0, 1, 1, 1, 3, 3, 3, 1, 1, 1},  
                                                           { 1, 0, 0, 0, 0, 1, 1, 6, 0, 0, 0, 1, 1, 1, 3, 0, 3, 3, 3, 1},        
                                                           { 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 4, 6, 4, 3, 3, 1}, 
                                                           { 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1}, 
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 1},
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 4, 4, 4, 3, 3, 3, 1},
                                                           { 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                                           { 0, 0, 0, 5, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                                           { 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}, true, true, true, true, false, false));
        
        templateList.Add(new LevelTemplate(new int[,] {    { 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1},  
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1},  
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 4, 4, 4, 0, 0, 0, 0, 1, 1},  
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 3, 1, 1, 1, 0, 0, 0, 1, 1, 1},  
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 3, 1, 1, 1, 0, 0, 0, 1, 1, 1},  
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 3, 1, 1, 1, 6, 0, 0, 0, 1, 1},  
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 3, 1, 1, 1, 1, 0, 0, 0, 1, 1},  
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1, 1},  
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 7, 0, 1, 1, 1, 1, 1, 0, 0, 1, 1},        
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 3, 1, 1, 1, 1, 0, 0, 0, 1, 1}, 
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 3, 0, 0, 1, 0, 0, 0, 0, 1, 1}, 
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 3, 0, 0, 0, 0, 0, 0, 1, 1, 1},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 3, 0, 0, 4, 4, 0, 1, 1, 1, 1},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1},
                                                           { 0, 0, 0, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0},
                                                           { 0, 0, 0, 4, 4, 4, 0, 0, 0, 1, 1, 0, 0, 0, 0, 4, 4, 0, 4, 0},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}, false, false, true, true, false, true));

        templateList.Add(new LevelTemplate(new int[,] {    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
                                                           { 0, 0, 0, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                                           { 0, 0, 0, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},  
                                                           { 0, 0, 0, 4, 4, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0},  
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 3, 1},  
                                                           { 1, 3, 3, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 3, 3, 1},  
                                                           { 1, 3, 3, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 3, 3, 1},  
                                                           { 1, 3, 3, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 1},  
                                                           { 1, 4, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 4, 1},        
                                                           { 1, 1, 1, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1}, 
                                                           { 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 1, 1, 1, 1}, 
                                                           { 1, 0, 0, 0, 1, 1, 0, 0, 0, 3, 3, 0, 0, 0, 4, 1, 1, 1, 0, 1},
                                                           { 1, 0, 0, 0, 0, 1, 1, 0, 3, 0, 0, 3, 0, 0, 1, 1, 1, 0, 0, 1},
                                                           { 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 3, 1, 1, 1, 0, 0, 0, 1},
                                                           { 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}, true, true, true, true, false, true));

        templateList.Add(new LevelTemplate(new int[,] {    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 0, 0, 0, 0, 0, 0},  
                                                           { 1, 0, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 4, 0, 1},  
                                                           { 1, 3, 0, 0, 3, 0, 0, 0, 0, 0, 0, 5, 6, 6, 0, 1, 1, 1, 1, 1},  
                                                           { 1, 1, 0, 0, 3, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},  
                                                           { 1, 1, 1, 0, 3, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},  
                                                           { 1, 1, 1, 0, 3, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1},  
                                                           { 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1},  
                                                           { 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 0, 0, 1, 1},        
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1}, 
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1}, 
                                                           { 1, 0, 0, 0, 0, 0, 0, 2, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1},
                                                           { 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1},
                                                           { 1, 0, 0, 0, 0, 4, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1},
                                                           { 0, 0, 0, 0, 4, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1},
                                                           { 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1},
                                                           { 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}, false, true, true, false, false, false));

        templateList.Add(new LevelTemplate(new int[,] {    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 1, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 1, 1, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 0, 0, 0, 1, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 4, 3, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 4, 1, 6, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 1, 1, 1, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},        
                                                           { 1, 2, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1}, 
                                                           { 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 3, 3, 0, 0, 0, 0, 0, 0, 1}, 
                                                           { 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 3, 3, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 1, 0, 0, 0, 0, 4, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
                                                           { 0, 0, 0, 0, 3, 3, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0},
                                                           { 0, 0, 6, 6, 3, 3, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1}}, false, true, true, true, true, false));

        templateList.Add(new LevelTemplate(new int[,] {    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1},  
                                                           { 0, 4, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 1, 1, 4, 0, 0, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 1, 1, 0, 0, 3, 3, 0, 0, 0, 0, 0, 0, 0, 1, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1},  
                                                           { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 1, 0, 0, 0, 0, 0, 0, 3, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1},  
                                                           { 1, 1, 0, 0, 0, 0, 0, 0, 3, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1},        
                                                           { 1, 1, 1, 0, 0, 0, 0, 2, 6, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1}, 
                                                           { 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1}, 
                                                           { 1, 1, 1, 3, 3, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
                                                           { 1, 1, 0, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1},
                                                           { 1, 1, 0, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1},
                                                           { 1, 1, 0, 7, 1, 0, 0, 0, 0, 3, 3, 0, 3, 3, 0, 0, 0, 0, 0, 0},
                                                           { 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0},
                                                           { 1, 1, 0, 1, 1, 1, 0, 0, 0, 6, 6, 0, 6, 6, 0, 6, 4, 0, 0, 0},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}, true, false, false, true, false, false));

        templateList.Add(new LevelTemplate(new int[,] {    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                                           { 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1},
                                                           { 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1},  
                                                           { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 3, 6, 3, 1, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 3, 0, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 3, 0, 3, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 6, 1, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1},  
                                                           { 1, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 2, 4, 0, 0, 0, 0, 1, 1, 1},        
                                                           { 1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1}, 
                                                           { 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1, 1, 3, 0, 0, 0, 0, 1, 1}, 
                                                           { 1, 0, 0, 1, 0, 3, 3, 3, 0, 0, 0, 1, 1, 0, 3, 0, 0, 0, 0, 1},
                                                           { 1, 1, 0, 1, 3, 3, 7, 3, 3, 0, 1, 1, 1, 1, 0, 3, 0, 0, 0, 1},
                                                           { 1, 0, 0, 1, 3, 3, 0, 3, 3, 0, 1, 1, 1, 1, 6, 3, 0, 0, 0, 1},
                                                           { 0, 0, 0, 1, 3, 0, 4, 0, 3, 3, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0},
                                                           { 0, 0, 1, 1, 1, 5, 1, 5, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0},
                                                           { 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 6, 0, 0, 0},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}, false, false, true, true, false, false));

        templateList.Add(new LevelTemplate(new int[,] {    { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0},  
                                                           { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},  
                                                           { 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 0, 1, 1},  
                                                           { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 4, 1, 1, 1, 1},  
                                                           { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1},  
                                                           { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 5, 1, 1, 1, 0, 1},  
                                                           { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 1},  
                                                           { 1, 1, 1, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},        
                                                           { 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1}, 
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1}, 
                                                           { 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 1, 0, 0, 0, 0, 0},
                                                           { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 3, 3, 3, 1, 0, 0, 0, 0, 0},
                                                           { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 4, 7, 4, 0, 0, 0, 0, 0, 0},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1}}, true, true, true, true, true, true));

        templateList.Add(new LevelTemplate(new int[,] {    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                                           { 0, 0, 0, 0, 0, 0, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},  
                                                           { 0, 0, 0, 0, 0, 0, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},  
                                                           { 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1},  
                                                           { 1, 0, 1, 0, 2, 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 3, 3, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 3, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1},        
                                                           { 1, 0, 0, 0, 0, 0, 3, 0, 6, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1}, 
                                                           { 1, 0, 0, 0, 0, 0, 0, 6, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 1}, 
                                                           { 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1},
                                                           { 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 4, 0, 0, 0, 1},
                                                           { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0},
                                                           { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 3, 3, 3, 0, 0, 0},
                                                           { 0, 0, 6, 6, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 3, 3, 3, 0, 0, 0},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}, true, true, true, true, false, false));

        templateList.Add(new LevelTemplate(new int[,] {    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 3, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 4, 4, 3, 1, 1, 0, 0, 1, 1, 3, 3, 1, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 3, 3, 3, 1, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 3, 3, 1, 1},  
                                                           { 1, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 7, 3, 1, 1},  
                                                           { 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 4, 3, 1, 4, 1, 1, 1},  
                                                           { 1, 0, 0, 3, 0, 0, 0, 0, 0, 3, 0, 3, 0, 1, 1, 1, 1, 1, 1, 1},  
                                                           { 1, 0, 0, 3, 0, 0, 0, 0, 3, 0, 3, 0, 3, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 1, 0, 0, 0, 0, 4, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 1},        
                                                           { 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 1}, 
                                                           { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1}, 
                                                           { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                                           { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                                           { 0, 0, 0, 2, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 6, 6, 6, 0, 0, 0},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1}}, false, false, true, true, true, true));

        templateList.Add(new LevelTemplate(new int[,] {    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 4, 4, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 3, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1},  
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},        
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 3, 3, 3, 0, 0, 1, 0, 0, 1}, 
                                                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 1, 0, 0, 1}, 
                                                           { 1, 0, 0, 3, 0, 3, 0, 3, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1},
                                                           { 1, 0, 0, 3, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 1, 0, 0, 3, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                                           { 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 3, 3, 0, 0, 0, 0, 0},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 3, 0, 0, 0, 0},
                                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 4, 4, 0, 5, 5, 0, 0},
                                                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}, true, false, true, true, false, false));



    }


    struct LevelTemplate
    {
        public int[,] levelPart;
        public bool upperLeftBound;
        public bool upperRightBound;
        public bool lowerLeftBound;
        public bool lowerRightBound;
        public bool downBound;
        public bool upBound;

        

        public LevelTemplate(int[,] a, bool uLeft, bool uRight, bool lLeft, bool lRight, bool down, bool up)
        {
            levelPart = a;
            upperLeftBound = uLeft;
            upperRightBound = uRight;
            lowerLeftBound = lLeft;
            lowerRightBound = lRight;
            downBound = down;
            upBound = up;
        }

        public LevelTemplate(int[,] a, LevelTemplate temp)
        {
            levelPart = a;
            upperLeftBound = temp.upperLeftBound;
            upperRightBound = temp.upperRightBound;
            lowerLeftBound = temp.lowerLeftBound;
            lowerRightBound = temp.lowerRightBound;
            downBound = temp.downBound;
            upBound = temp.upBound;
        }


        

        public Vector3 GetSpawnPoint(){
            for (int i = 0; i < levelPart.GetUpperBound(0); i++ )
            {
                for (int j = 0; j < levelPart.GetUpperBound(1); j++)
                {
                    if (levelPart[i, j] == 2)
                        return new Vector3(i, j, 0);
                }
            }
            return Vector3.zero;
        }

        public bool HasRightWay()
        {
            return upperRightBound || lowerRightBound;
        }

        public bool CanCrossRightTo(LevelTemplate a)
        {
            return upperRightBound && a.upperLeftBound || lowerRightBound && a.lowerLeftBound;
        }

        public bool HasLeftWay()
        {
            return upperLeftBound || lowerLeftBound;
        }

        public bool HasUpWay()
        {
            return upBound;
        }

        public bool HasDownWay()
        {
            return downBound;
        }

        public int GetXBound(){
            if (levelPart != null)
            {
                return levelPart.GetUpperBound(0);
            }
            else
            {
                return -1;
            }
        }

        public int GetYBound()
        {
            if (levelPart != null)
            {
                return levelPart.GetUpperBound(1);
            }
            else
            {
                return -1;
            }
        }

    }
}
