using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockGenerator : MonoBehaviour {

    GameObject parent;
    
    public Transform noRoundness;
    public Transform leftRoundness;
    public Transform rightRoundness;
    public Transform fillerNoCollider;
    public Transform fillerWithCollider;
    public Transform crystal;
    public Transform medPack;
    public Transform fireTrap;
    public Transform spikeTrap;
    public Transform empty;
    public int[,] testArray = new int[,] {    { 1, 1, 1, 1, 1 }, 
                                              { 1, 0, 0, 1, 1 }, 
                                              { 1, 0, 1, 1, 1 }, 
                                              { 0, 0, 0, 1, 1 },
                                              { 0, 1, 1, 1, 0 }};
    float offsetX;
    float offsetY;
    
	// Use this for initialization
	void Start () {
       // testArray = RotateArrayClockwise(testArray);
       // SpawnLevel(testArray);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    bool TestPrefabs()
    {
        return noRoundness != null && leftRoundness != null && rightRoundness != null && fillerNoCollider != null && fillerWithCollider != null && crystal != null && medPack != null && fireTrap != null && spikeTrap != null;
    }

    public Vector3 GridToWorldPosition(Vector3 a)
    {
        return new Vector3(a.x * 3.1f - offsetX, a.y * 3.1f - offsetY, 0);
    }

    

    public void SpawnLevel(int[,] level, bool useObjects = true)
    {
        if (empty == null)
        {
            empty = new GameObject().transform;
        }
        if (!GameObject.Find("Environment"))
        {
            parent = new GameObject();
            parent.name = "Environment";
        }
        else
        {
            parent = GameObject.Find("Environment");
        }
        offsetX = level.GetUpperBound(0) * 3.1f / 2 + 3.1f / 2;
        offsetY = level.GetUpperBound(1) * 3.1f / 2 + 3.1f / 2;
        string name;
        if (TestPrefabs() && parent != null)
        {
            for (int x = 0; x <= level.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= level.GetUpperBound(1); y++)
                {
                    Transform temp = null;
                    float tilePosX = x * 3.1f - offsetX;
                    float tilePosY = y * 3.1f - offsetY;
                    name = string.Format("Ground[{0}][{1}]", (float)System.Math.Round(tilePosX, 1), (float)System.Math.Round(tilePosY, 1));
                    if (level[x,y] == 1)
                    {
                        int check = checkNeighbours(level, x, y);
                        switch (check)
                        {
                            case 0:
                                // Standard Block, ohne Abrundungen
                                temp = (Transform)Instantiate(noRoundness, new Vector3(tilePosX, tilePosY, 0f), Quaternion.identity);
                                break;
                            case 1:
                                // Block mit linker Abrundung
                                temp = (Transform)Instantiate(leftRoundness, new Vector3(tilePosX, tilePosY, 0f), Quaternion.identity);
                                break;
                            case 2:
                                // Block mit rechter Abrundung
                                temp = (Transform)Instantiate(rightRoundness, new Vector3(tilePosX, tilePosY, 0f), Quaternion.identity);
                                break;
                            case 3:
                                // Füller-Block mit Collider
                                temp = (Transform)Instantiate(fillerWithCollider, new Vector3(tilePosX, tilePosY, 0f), Quaternion.identity);
                                break;
                            case 4:
                                // Füller-Block ohne Collider
                                temp = (Transform)Instantiate(fillerNoCollider, new Vector3(tilePosX, tilePosY, 0f), Quaternion.identity);
                                break;
                            default:
                                temp = (Transform)Instantiate(noRoundness, new Vector3(tilePosX, tilePosY, 0f), Quaternion.identity);
                                break;

                        }
                        
                        

                    }else
                        if (level[x, y] == 3 && useObjects)  // Kristall
                    {
                        temp = (Transform)Instantiate(crystal, new Vector3(tilePosX, tilePosY, 0f), Quaternion.identity);
                    }else
                    if (level[x, y] == 4 && useObjects) // Feuerfalle
                    {
                        temp = (Transform)Instantiate(fireTrap, new Vector3(tilePosX, tilePosY, 0f), Quaternion.identity);
                    }else
                    if (level[x, y] == 5 && useObjects) // Stachelfalle
                    {
                        temp = (Transform)Instantiate(spikeTrap, new Vector3(tilePosX, tilePosY, 0f), Quaternion.identity);
                    }else
                    if (level[x, y] == 6 && useObjects) // Beliebige Falle
                    {
                        if (Random.Range(0, 1) > 0.5)
                        {
                            temp = (Transform)Instantiate(fireTrap, new Vector3(tilePosX, tilePosY, 0f), Quaternion.identity);
                        }
                        else
                        {
                            temp = (Transform)Instantiate(spikeTrap, new Vector3(tilePosX, tilePosY, 0f), Quaternion.identity);
                        }

                    }else
                    if (level[x, y] == 7 && useObjects) // Medpack
                    {
                        temp = (Transform)Instantiate(medPack, new Vector3(tilePosX, tilePosY, 0f), Quaternion.identity);
                    }
                    if (temp != null)
                    {
                        if (level[x, y] == 3)
                        {
                            temp.tag = "ScoreObject";
                        }
                        else
                        {
                            temp.tag = "Clone";
                        }
                        temp.parent = parent.transform;
                        temp.name = name;

                    }
                    if (level[x, y] == 10) // Player
                    {
                        temp = (Transform)Instantiate(empty, new Vector3(tilePosX, tilePosY, 0f), Quaternion.identity);
                        temp.tag = "PlayerSpawn";
                        temp.name = name;
                        temp.parent = parent.transform;
                    }
                    if (level[x, y] == 11) // Flag
                    {
                        temp = (Transform)Instantiate(empty, new Vector3(tilePosX + 1.2f, tilePosY + 4.3f, 0f), Quaternion.identity);
                        temp.tag = "FlagSpawn";
                        temp.name = name;
                        temp.parent = parent.transform;
                    }
                    if (level[x, y] == 12) // AI
                    {
                        temp = (Transform)Instantiate(empty, new Vector3(tilePosX, tilePosY, 0f), Quaternion.identity);
                        temp.tag = "AISpawn";
                        temp.name = name;
                        temp.parent = parent.transform;
                    }

                    
                }
            }
        }
        else
        {
            Debug.Log("Keine/Nicht genug prefabs im BlockGenerator gesetzt!");
        }

    }

    private int checkNeighbours(int[,] level, int x, int y)
    {
        // Standard Block, ohne Abrundungen 0
        if (checkTop(level, x, y) == 0 && (checkLeft(level, x, y) == 1 && checkRight(level, x, y) == 1 ||
                                            (checkLeft(level, x, y) == 0 && checkRight(level, x, y) == 0)))
        {
            return 0;
        }

        // Block mit linker Abrundung 1
        if (checkTop(level, x, y) == 0 && checkLeft(level, x, y) == 0 && checkRight(level, x, y) == 1)
        {
            return 1;
        }
        
             
        // Block mit rechter Abrundung 2
        if (checkTop(level, x, y) == 0 && checkRight(level, x, y) == 0 && checkLeft(level, x, y) == 1)
        {
            return 2;
        }

        // Füller-Block mit Collider 3
        if (checkTop(level, x, y) == 1 && checkLeft(level, x, y) == 0 || checkRight(level, x, y) == 0 || checkBottom(level, x, y) == 0)
        {
            return 3;
        }

        // Füller-Block ohne Collider 4
        if (checkTop(level, x, y) == 1 && checkLeft(level, x, y) == 1 && checkRight(level, x, y) == 1 && checkBottom(level, x, y) == 1)
        {
            return 4;
        }
        return -1;
    }

    private int checkTop(int[,] level, int x, int y)
    {
        if (y < level.GetUpperBound(1))
        {
            return level[x,y+1] == 1 ? 1 : 0;
        } 
        else{
            return 0;
        } 
    }

    private int checkLeft(int[,] level, int x, int y)
    {
        if (x > 0)
        {
            return level[x-1,y] == 1 ? 1: 0;
        }
        else{
            return 0;
        }
    }

    private int checkRight(int[,] level, int x, int y)
    {
        if (x < level.GetUpperBound(0))
        {
            return level[x+1,y] == 1 ? 1: 0;
        }
        else{
            return 0;
        }
    }

    private int checkBottom(int[,] level, int x, int y)
    {
         if (y > 0)
        {
            return level[x,y-1] == 1 ? 1 : 0;
        } 
        else{
            return 0;
        } 
    }

    public int[,] RotateArrayClockwise(int[,] a)
    {
        int height = a.GetUpperBound(1) + 1;
        int width = a.GetUpperBound(0) + 1;
        int[,] b = new int[width, height];
       // Debug.Log("y: " + height + " x: " + width);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                b[x, y] = a[width-y-1, x];
            }
        }

        return b;
    }
}
