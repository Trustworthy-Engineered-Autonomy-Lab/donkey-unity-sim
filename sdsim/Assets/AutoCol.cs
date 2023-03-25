using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCol : MonoBehaviour
{

    private int hits;
    private int lapNum;
    private int hitsCurrentLap;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Finish") == true)
        {
            lapNum = lapNum + 1;
            hitsCurrentLap = 0;
            Debug.Log("Lap: " + lapNum + " |Total hits: " + hits);
        }
        else
        {
            hits = hits + 1;
            hitsCurrentLap = hitsCurrentLap + 1;
            Debug.Log("Lap: " + lapNum + " |Hits this lap: " + hitsCurrentLap + " |Total hits: " + hits);
        }
        
    }
}
