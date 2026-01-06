using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestElbowForce : MonoBehaviour
{
    public float torque;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ArticulationBody articulationBody = GetComponent<ArticulationBody>();
            var xDrive = articulationBody.xDrive;
            xDrive.driveType = ArticulationDriveType.Target;
            xDrive.target = 0f;
            articulationBody.xDrive = xDrive;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            ArticulationBody articulationBody = GetComponent<ArticulationBody>();
            var xDrive = articulationBody.xDrive;
            xDrive.driveType = ArticulationDriveType.Force;
            xDrive.target = 0;
            articulationBody.xDrive = xDrive;
            articulationBody.AddTorque(new Vector3(-torque, 0, -torque));
        }
        float angle = transform.rotation.eulerAngles.x;
        //Debug.Log(angle > 90 ? 360f - angle : -angle);
        //Debug.Log(transform.rotation.eulerAngles);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Wall")
        {
            Debug.Log("oh");
        }
    }
}
