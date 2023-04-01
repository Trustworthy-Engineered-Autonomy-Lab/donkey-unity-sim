using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCol : MonoBehaviour
{

    private int hits;
    private int lapNum;
    private int hitsCurrentLap;
    private int resetCount;

    private float resetTime = 3;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Finish") == true)
        {
            lapNum += 1;
            hitsCurrentLap = 0;
            Debug.Log("Lap: " + lapNum + " |Total hits: " + hits);
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

            if (resetTime <= 0)
            {
                Debug.Log("Collision lasted 3 secs");
                FindObjectOfType<moveToFinish>().updatePos(1);
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
