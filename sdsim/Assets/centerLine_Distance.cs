using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation; 

public class centerLine_Distance : MonoBehaviour
{

    public float dist = Mathf.Infinity;
    public GameObject obj;
    public int t;
    public VertexPath vertexPath;
    // Start is called before the first frame update
    void Start()
    {
        vertexPath = new VertexPath();
    }

    // Update is called once per frame
    void Update()
    {
        t = 1;

        if (vertexPath != null)
        {
            dist = Vector3.Distance(transform.position, vertexPath.GetPoint(t));
            Debug.DrawLine(transform.position, vertexPath.GetPoint(t), Color.green);
        }
    }
}
