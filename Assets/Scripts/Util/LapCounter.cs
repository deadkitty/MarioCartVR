using UnityEngine;
using System.Collections;

public class LapCounter : MonoBehaviour 
{
    public FinishLineListener[] finishLines;

    public int maxLaps = 5;

    private int lap = 0;

    private int lineIndex = 0;

    private bool raceFinished = false;

    private string finishedString = "";

    void Start()
    {

    }

    void Update()
    {

    }

    public void TriggerEntered(FinishLineListener listener)
    {
        if (finishLines[lineIndex].lineNumber != listener.lineNumber)
        {
            ++lineIndex;
            lineIndex %= finishLines.Length;
            finishLines[lineIndex] = listener;
        }

        if (finishLines[(lineIndex + 1) % finishLines.Length].lineNumber == 0 &&
            finishLines[(lineIndex + 2) % finishLines.Length].lineNumber == 1 &&
            finishLines[(lineIndex + 3) % finishLines.Length].lineNumber == 2)
        {
            ++lap;
            for (int i = 0; i < finishLines.Length; ++i)
            {
                finishLines[i] = finishLines[0];
            }

            if(lap == maxLaps)
            {
                networkView.RPC("RaceFinished", RPCMode.AllBuffered, null);
            }
        }
    }

    [RPC]
    void RaceFinished()
    {
        if(networkView.isMine)
        {
            finishedString = "You Lost the Race!!!";
        }
        else
        {
            finishedString = "You Won the Race!!!";
        }

        raceFinished = true;
    }

    void OnGUI()
    {
        GUI.TextField(new Rect(50, 50, 100, 30), "Lap: " + lap);

        if (raceFinished)
        {
            GUI.TextField(new Rect(50, 150, 200, 30), finishedString);
        }
    }
}