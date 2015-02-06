using UnityEngine;
using System.Collections;

public class ToggleCamera : MonoBehaviour 
{
    public GameObject riftCamera;
    public GameObject normalCamera;

    public bool useRift = true;

    void Start () 
    {
	    if(useRift)
        {
            riftCamera.SetActive(true);
            normalCamera.SetActive(false);
        }
        else
        {
            riftCamera.SetActive(false);
            normalCamera.SetActive(true);
        }
	}
}
