using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPhys : MonoBehaviour
{   
    WheelCollider wc;
    float originalForwardStiffness;
    float originalSidewaysStiffness;
    public float frictionScale = 1.0f; //Addition for friction scale

    void Awake()
    {
        wc = gameObject.GetComponent<WheelCollider>();
        originalForwardStiffness = wc.forwardFriction.stiffness;
        originalSidewaysStiffness = wc.sidewaysFriction.stiffness;
    }

    void FixedUpdate()
    {
        WheelHit hit;
        if (wc.GetGroundHit(out hit))
        {
            WheelFrictionCurve fFriction = wc.forwardFriction;
            fFriction.stiffness = hit.collider.material.staticFriction * originalForwardStiffness * frictionScale; //Multiply by frictionScale for friction loss
            wc.forwardFriction = fFriction;

            WheelFrictionCurve sFriction = wc.sidewaysFriction;
            sFriction.stiffness = hit.collider.material.staticFriction * originalSidewaysStiffness * frictionScale; //Multiply by frictionScale for friction loss
            wc.sidewaysFriction = sFriction;
        }        
    }
}
