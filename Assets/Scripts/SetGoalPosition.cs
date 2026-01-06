using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGoalPosition : MonoBehaviour
{
    public void SetPosition(float initialAngle)
    {
        //Debug.Log("Set Goal Position");
        var xDrive = GetComponent<ArticulationBody>().xDrive;
        xDrive.target = initialAngle;
        GetComponent<ArticulationBody>().xDrive = xDrive;
    }
}
