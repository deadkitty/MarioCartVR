using UnityEngine;
using System.Collections;

public class Players : MonoBehaviour
{
    private static Players sInstance;

    public const int playerCount = 2;
    public GameObject[] player = new GameObject[playerCount];
    
	void Start () 
    {
        sInstance = this;
	}

    public static void SetPlayer(int index, GameObject player)
    {
        sInstance.player[index] = player;
        sInstance.player[index].name = "Cart" + index;
    }

    public static GameObject GetPlayer(int index)
    {
        return sInstance.player[index];
    }
}
