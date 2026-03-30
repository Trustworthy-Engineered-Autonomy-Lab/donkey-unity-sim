using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveToFinish : MonoBehaviour
{
    public PathManager pathManager;

    public void updatePos(int isColliding)
    {
        if (isColliding == 1)
        {
            // End the car's run if the randomStart flag is set to true
            if (GlobalState.randomStart)
            {
                Car car = GetComponent<Car>();
                if (car != null){
                    car.last_collision = "barrier";
                }
                /*
                pathManager = GameObject.FindObjectOfType<PathManager>();
                int total = pathManager.carPath.centerNodes.Count;
                int randIdx = UnityEngine.Random.Range(0,total - 1);
                Vector3 randStartPos = pathManager.carPath.centerNodes[randIdx].pos;
                randStartPos.y += 0.1f;
                transform.position = randStartPos;
                transform.rotation = pathManager.carPath.centerNodes[randIdx].rotation;*/

            }
            
            else{

                //for mini monaco
                transform.position = new Vector3(13.03789f, 0.5591203f, -68.18274f);
                transform.rotation = Quaternion.Euler(-0.007f, -136.209f, 0.005f);
            

                //for mountain track
                //transform.position = new Vector3(50.07488f, 5.234812f, 51.09399f);
                //transform.rotation = Quaternion.Euler(1.625f, 178.326f, 0.116f);

                //for circuit_launch
                //transform.position = new Vector3(20.28229f, 0.5591203f, -22.11876f);
                //transform.rotation = Quaternion.Euler(-0.007f, -90f, 0.005f);
            }
        }
    }
}
