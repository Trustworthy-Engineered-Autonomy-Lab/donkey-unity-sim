using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCol : MonoBehaviour
{

    private int hits;
    private int lapNum;
    private int hitsCurrentLap;
    private int resetCount;
    private float timeThisLap;
    private float lapStartTime;

    private Vector3 lastPos;
    private Vector3 currentPos;

    private float resetTime = 3;

    void OnTriggerEnter(Collider other)
    {   
        currentPos = gameObject.transform.position;
        if(other.gameObject.tag.Equals("Finish") == true)
        {
            lapNum += 1;
            hitsCurrentLap = 0;
            Debug.Log("Lap: " + lapNum + " |Majors: " + resetCount + " |Minors: " + (hits - resetCount));
            lapStartTime = Time.time;
        }
        else
        {
            hits += 1;
            hitsCurrentLap = hitsCurrentLap + 1;
            Debug.Log("Lap: " + lapNum + " |Hits this lap: " + hitsCurrentLap + " |Majors: " + resetCount + " |Minors: " + (hits-resetCount));
        }
        
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Finish") == false)
        {
 
            if (resetTime > 0)
            {
                resetTime -= Time.deltaTime;  
            }

            else
            {
                timeThisLap = Time.time - lapStartTime - 3;
                Debug.Log("Reset. Lap Lasted: " + timeThisLap);
                // Debug.Log("Kshitij Position: " + gameObject.transform.position);
                
                FindObjectOfType<moveToFinish>().updatePos(1);
                // Debug.Log("Kshitij Position: " + gameObject.transform.position);
                
                resetTime = 3;
                resetCount += 1;
            }
        }
    }

    void OnTriggerLeave(Collider other)
    {
        resetTime = 3;
    }
    
}

