using UnityEngine;
using System.Collections;

public class CartTimer : MonoBehaviour
{
    private static CartTimer sInstance;

    public float timeTillRaceStarts = 0.0f;
    private float currentTime = 0.0f;
    private float timeLastFrame = -1.0f;

    private static bool startTimer = false;
    private static bool beginRace = false;
    
    public static float CurrentTime
    {
        get { return sInstance.currentTime; }
        set { sInstance.currentTime = value; }
    }

    public static bool BeginRace
    {
        get { return beginRace; }
    }

	void Start () 
    {
        sInstance = this;
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
}
