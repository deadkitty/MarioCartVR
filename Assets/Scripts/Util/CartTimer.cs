using UnityEngine;
using System.Collections;

public class CartTimer : MonoBehaviour
{
    private static float currentTime = 0.0f;
    private static float timeTillRaceStarts = 0.0f;
    private static float timeLastFrame = -1.0f;

    private static bool startTimer = false;

    private static bool beginRace = false;

    public static bool BeginRace
    {
        get { return beginRace; }
    }

	void Start () 
    {
        currentTime = -timeTillRaceStarts;
	}
	
	void Update () 
    {
        if (!startTimer)
            return;

        currentTime += Time.deltaTime;

        if(currentTime >= 0.0f && timeLastFrame < 0.0f)
        {
            beginRace = true;
        }

        timeLastFrame = currentTime;
	}

    public static void StartRaceTimer()
    {
        startTimer = true;
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(200, 50, 100, 30), "Time: " + currentTime);
    }
}
