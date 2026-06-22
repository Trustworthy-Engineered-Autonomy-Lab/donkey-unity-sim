using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPhys : MonoBehaviour
{   
    WheelCollider wc;
    WheelFrictionCurve originalForwardFriction;
    WheelFrictionCurve originalSidewaysFriction;
    public float frictionScale = 1.0f; //Addition for friction scale

    void Awake()
    {
        wc = gameObject.GetComponent<WheelCollider>();
        originalForwardFriction = wc.forwardFriction;
        originalSidewaysFriction = wc.sidewaysFriction;
        ApplyFrictionScale();
    }

    public void SetFrictionScale(float scale)
    {
        frictionScale = Mathf.Max(0.0f, scale);
        ApplyFrictionScale();
    }

    public float GetFrictionScale()
    {
        return frictionScale;
    }

    void ApplyFrictionScale(float surfaceScale = 1.0f)
    {
        float scale = Mathf.Clamp01(frictionScale) * Mathf.Max(0.01f, surfaceScale);

        WheelFrictionCurve fFriction = originalForwardFriction;
        fFriction.stiffness = originalForwardFriction.stiffness * scale;
        fFriction.extremumValue = originalForwardFriction.extremumValue * scale;
        fFriction.asymptoteValue = originalForwardFriction.asymptoteValue * scale;
        wc.forwardFriction = fFriction;

        WheelFrictionCurve sFriction = originalSidewaysFriction;
        sFriction.stiffness = originalSidewaysFriction.stiffness * scale;
        sFriction.extremumValue = originalSidewaysFriction.extremumValue * scale;
        sFriction.asymptoteValue = originalSidewaysFriction.asymptoteValue * scale;
        wc.sidewaysFriction = sFriction;
    }

    void FixedUpdate()
    {
        WheelHit hit;
        if (wc.GetGroundHit(out hit))
        {
            float surfaceScale = 1.0f;
            if (hit.collider != null && hit.collider.material != null)
            {
                surfaceScale = hit.collider.material.staticFriction;
            }
            ApplyFrictionScale(surfaceScale);
        }        
    }
}
