using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathCreation.Examples
{
    public class centerLine_Distance : MonoBehaviour
    {

        public PathCreator pathCreator;
        public float dist = 0;
        float distanceTravelled;
        public float cAngle;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (pathCreator != null)
            {

                Vector3 closestPoint = pathCreator.path.GetClosestPointOnPath(transform.position);

                Vector3 closestPointXZ = new Vector3(closestPoint.x, 0.1f, closestPoint.z);

                Vector3 carPosXZ = new Vector3(transform.position.x, 0.1f, transform.position.z);

                dist = Vector3.Distance(closestPointXZ, carPosXZ);

                Vector3 trackVec = pathCreator.path.GetNormal(pathCreator.path.GetClosestTimeOnPath(closestPoint));
                    
                Vector3 carVec = transform.GetComponent<Rigidbody>().velocity;

                if(carVec.magnitude < 0.5)
                {
                    this.cAngle = 0;
                }
                else
                {
                    this.cAngle = Vector3.Angle(trackVec, carVec) - 90f;
                }
            }
        }
    }
}
