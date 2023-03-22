using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCol : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit!");
    }
}
