using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightIntensity : MonoBehaviour
{
    private Light _myLight;
    // Start is called before the first frame update
    void Start()
    {
        _myLight = this.GetComponent<Light>();
        _myLight.intensity = GlobalState.lightIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
