using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCol : MonoBehaviour
{

    private int hits;
    private int lapNum;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Finish") == true)
        {
            lapNum = lapNum + 1;
            Debug.Log("Lap: " + lapNum);
        }
        else
        {
            hits = hits + 1;
            Debug.Log("Hit! Count: " + hits);
        }
        
    }
}
