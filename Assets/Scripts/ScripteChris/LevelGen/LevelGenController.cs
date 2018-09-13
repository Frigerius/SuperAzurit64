using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(MapGeneratorCA))]
[RequireComponent(typeof(Templates))]


public class LevelGenController : MonoBehaviour {


    public bool playLevelOnStart = true;
    public bool spawnTraps = true;
    bool isLevelBuilt = false;
    public MapGeneratorCA cellularGen;
    public Templates templateScript;
    

    void Start()
    {



        if (playLevelOnStart && GameObject.FindGameObjectWithTag("PlayerSpawn") != null && GameObject.FindGameObjectWithTag("Goal") != null)
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("PlayerSpawn").transform.position;

            GameObject.FindGameObjectWithTag("Goal").transform.position = GameObject.FindGameObjectWithTag("FlagSpawn").transform.position;

        }

    }

    public void GenerateCaveLevel()
    {
        if (cellularGen == null)
        {
            cellularGen = GetComponent<MapGeneratorCA>();
        }
        cellularGen.GenerateMap();
        isLevelBuilt = true;
    }


    public void GenerateTemplateLevel()
    {
        if (templateScript == null)
        {
            templateScript = GetComponent<Templates>();
        }
        templateScript.ClearTemplateList();
        templateScript.TestLevel(spawnTraps);
        isLevelBuilt = true;
        
            

        
    }

    public bool IsLevelBuilt()
    {
        return isLevelBuilt;
    }

    public void DeleteLevel()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Clone");
        foreach (GameObject obj in objects)
        {
            DestroyImmediate(obj);
        }
        objects = GameObject.FindGameObjectsWithTag("ScoreObject");
        foreach (GameObject obj in objects)
        {
            DestroyImmediate(obj);
        }
        objects = GameObject.FindGameObjectsWithTag("FlagSpawn");
        foreach (GameObject obj in objects)
        {
            DestroyImmediate(obj);
        }
        objects = GameObject.FindGameObjectsWithTag("PlayerSpawn");
        foreach (GameObject obj in objects)
        {
            DestroyImmediate(obj);
        }
        objects = GameObject.FindGameObjectsWithTag("AISpawn");
        foreach (GameObject obj in objects)
        {
            DestroyImmediate(obj);
        }
        isLevelBuilt = false;
    }

    public List<Vector3> GetAiSpawns()
    {
        List<Vector3> spawns = new List<Vector3>();
        GameObject[] temp = GameObject.FindGameObjectsWithTag("AISpawn");
        foreach(GameObject obj in temp){
            spawns.Add(obj.transform.position);
        }
        return spawns;
    }

    public Vector3 PlayerSpawn()
    {
        return GameObject.FindGameObjectWithTag("PlayerSpawn").transform.position;
    }

    
	
}
