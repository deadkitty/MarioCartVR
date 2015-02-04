using UnityEngine;
using System.Collections;

public class LapCounter : MonoBehaviour 
{
    private static LapCounter sInstance;

    public FinishLineListener[] finishLines;

    public int maxLaps = 5;
    private int currentLap = 0;
    private int lineIndex = 0;

    public static bool raceFinished = false;
    
    public static int MaxLaps
    {
        get { return sInstance.maxLaps; }
        set { sInstance.maxLaps = value; }
    }

    public static int CurrentLap
    {
        get { return sInstance.currentLap; }
        set { sInstance.currentLap = value; }
    }
    
    void Start()
    {
        sInstance = this;
    }

    public static void TriggerEntered(FinishLineListener listener, GameObject player)
    {
        if(player.networkView.isMine)
        {
            if (sInstance.finishLines[sInstance.lineIndex].lineNumber != listener.lineNumber)
            {
                ++sInstance.lineIndex;
                sInstance.lineIndex %= sInstance.finishLines.Length;
                sInstance.finishLines[sInstance.lineIndex] = listener;
            }

            if (sInstance.finishLines[(sInstance.lineIndex + 1) % sInstance.finishLines.Length].lineNumber == 0 &&
                sInstance.finishLines[(sInstance.lineIndex + 2) % sInstance.finishLines.Length].lineNumber == 1 &&
                sInstance.finishLines[(sInstance.lineIndex + 3) % sInstance.finishLines.Length].lineNumber == 2)
            {
                ++sInstance.currentLap;
                for (int i = 0; i < sInstance.finishLines.Length; ++i)
                {
                    sInstance.finishLines[i] = sInstance.finishLines[0];
                }

                if (CurrentLap == MaxLaps && !raceFinished)
                {
                    sInstance.RaceFinished(true);
                    sInstance.networkView.RPC("RaceFinished", RPCMode.OthersBuffered, false);
                }
            }
        }
    }

    public static void Reset()
    {
        CurrentLap = 0;
        raceFinished = false;
    }

    [RPC]
    void RaceFinished(bool wonRace)
    {
        if (wonRace)
        {
            MenuController.ShowMenu(Menu.EType.popupWin);
        }
        else
        {
            MenuController.ShowMenu(Menu.EType.popupLost);
        }

        raceFinished = true;
    }
}