﻿using UnityEngine;
using System.Collections;

public class FinishLineListener : MonoBehaviour 
{
    private LapCounter lapCounter;

    public int lineNumber;

	void Start () 
    {
        lapCounter = gameObject.GetComponentInParent<LapCounter>();
	}
	    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTrigger Enter: " + gameObject.name);

        lapCounter.TriggerEntered(this, other.gameObject);
    }
}
