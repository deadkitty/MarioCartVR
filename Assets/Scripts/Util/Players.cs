using UnityEngine;
using System.Collections;

public class Players : MonoBehaviour 
{
    public const int playerCount = 2;

    public GameObject[] player = new GameObject[playerCount];

    private static Players sInstance;

    public static void SetPlayer(int index, GameObject player)
    {
        sInstance.player[index] = player;
    }

    public static GameObject GetPlayer(int index)
    {
        return sInstance.player[index];
    }

	void Start () 
    {
        sInstance = this;
	}
}
