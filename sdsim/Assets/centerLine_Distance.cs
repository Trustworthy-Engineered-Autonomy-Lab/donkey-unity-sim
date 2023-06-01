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

        // Start is called before the first frame update
        void Start()
        {
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (pathCreator != null)
            {

                Vector3 closestPoint = pathCreator.path.GetClosestPointOnPath(transform.position);
                dist = Vector3.Distance(transform.position, closestPoint);
                Debug.DrawLine(transform.position, closestPoint, Color.red);
            }
        }

        void OnPathChanged()
        {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}
