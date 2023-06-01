using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation; 

public class centerLine_Distance : MonoBehaviour
{

    public float dist = Mathf.Infinity;
    public GameObject obj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        dist = Vector3.Distance(transform.position, obj.transform.position);
        Debug.DrawLine(transform.position, obj.transform.position, Color.green);
        
    }
}
