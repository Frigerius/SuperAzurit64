using UnityEngine;
using System.Collections;

public class LevelTester : MonoBehaviour, IJumpRequest
{

    AIMemory aiMem;
    AIMovement aiMovement;
    public int WindowX = 200;
    public int WindowY = 100;
    public bool calc = true;
    float nextCalc = 0;
    private int lastWinX;
    private int lastWinY;

    void Start()
    {
        aiMem = new AIMemory(WindowX, WindowY, transform.position, new EnvironmentAnalyser(), this, 3.1f);
        Camera.main.GetComponent<DebugGraph>().AiMapGraph = aiMem;
        aiMem.UpdateMemory(transform.position);
        lastWinX = WindowX;
        lastWinY = WindowY;
    }

    public bool CanYouJumpThis(Vector2 a, Vector2 b)
    {
        if (Mathf.Abs(a.x - b.x) <= 7 && Mathf.Abs(a.y - b.y) <= 1)
        {
            return true;
        }
        if (Mathf.Abs(a.x - b.x) <= 6 && Mathf.Abs(a.y - b.y) <= 3)
        {
            return true;
        }
        if (Mathf.Abs(a.x - b.x) <= 5 && Mathf.Abs(a.y - b.y) <= 4)
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        if (calc)
        {
            if (Time.time >= nextCalc)
            {
                if(lastWinX != WindowX || lastWinY != WindowY)
                {
                    lastWinX = WindowX;
                    lastWinY = WindowY;
                    RenewMem();
                }
                nextCalc = Time.time + 1f;
                aiMem.UpdateMemory(transform.position);
            }
        }
    }

    void RenewMem()
    {
        aiMem = new AIMemory(WindowX, WindowY, transform.position, new EnvironmentAnalyser(), this, 3.1f);
        Camera.main.GetComponent<DebugGraph>().AiMapGraph = aiMem;
    }
    
}
