using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveToFinish : MonoBehaviour
{
    public void updatePos(int isColliding)
    {
        if (isColliding == 1)
        {
            transform.position = new Vector3(13.03789f, 0.5591203f, -68.18274f);
            transform.rotation = Quaternion.Euler(-0.007f, -136.209f, 0.005f);
        }
    }
}
