using UnityEngine;
using System.Collections;

public class FinishLineListener : MonoBehaviour 
{
    public int lineNumber;
    	    
    void OnTriggerEnter(Collider other)
    {
        LapCounter.TriggerEntered(this, other.gameObject);
    }
}
