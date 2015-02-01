using UnityEngine;
using System.Collections;

public class CartTimer : MonoBehaviour
{
    private static float currentTime = -5.0f;
    private static float timeTillRaceStarts = 5.0f;
    private static float timeLastFrame = 0.0f;

    //private static float fastestLap = 0.0f;
    //private static int fastestLapIndex = 0;
    //private static int fastestLapPlayer = 0;

    //private static int playerIndex = 0;

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
        Debug.Log("CartTime.StartRaceTimer");
        startTimer = true;
    }

    public static void LapFinished(float lapTime, int lap, int playerIndex)
    {
        //if(lapTime < fastestLap)
        //{
        //    fastestLap = lapTime;
        //    fastestLapIndex = lap;
        //    fastestLapPlayer = playerIndex;
        //}
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(200, 50, 100, 30), "Time: " + currentTime);
    }
}
