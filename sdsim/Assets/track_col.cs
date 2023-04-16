using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class track_col : MonoBehaviour
{
    void onTriggerLeave(Collider other)
    {
        Debug.Log("Left track");
    }
}
