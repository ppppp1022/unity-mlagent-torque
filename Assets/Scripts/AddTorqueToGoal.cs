using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTorqueToGoal : MonoBehaviour
{
    [SerializeField] private TestChangeParameter shoulder_Test;
    [SerializeField] private ChangeParameter shoulder;
    [SerializeField] private ChangeParameterWrist_Env shoulder_wrist;
    [SerializeField] private int type = 0;
    [SerializeField] private ArticulationBody articulationBody;
    private float pastAngle;
    private float timer=0f;
    private bool hasTorque = false;
    public bool hasSet = false;
    public int updateGap = 0;
    public float gap = 200;
    public void Add(float torque)
    {
        var xDrive = articulationBody.xDrive;
        xDrive.driveType = ArticulationDriveType.Force;
        articulationBody.xDrive = xDrive;

        Debug.Log($"torque: {torque}");
        articulationBody.AddTorque(new Vector3(-torque, 0, -torque));
        hasTorque = true;
    }
    public void ResetVariable()
    {
        //Debug.Log("Reset Variables");
        pastAngle = 2000;
        timer = 0f;
        hasTorque = false;
        hasSet = false;
        updateGap = 0;
        gap = 2000;

        var xDrive = articulationBody.xDrive;
        xDrive.driveType = ArticulationDriveType.Target;
        xDrive.target = 0;
        articulationBody.xDrive = xDrive;
    }
    public IEnumerator SetPosition(float initialAngle)
    {
        //Debug.Log("waiting");

        var xDrive = GetComponent<ArticulationBody>().xDrive;
        xDrive.target = initialAngle;
        GetComponent<ArticulationBody>().xDrive = xDrive;
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => articulationBody.velocity.magnitude <= 0.001f);
        if (type == 0)
        {
            shoulder.enabled = true;
            shoulder.Setting();
        }
        else if (type == 1)
        {
            shoulder_Test.enabled = true;
            shoulder_Test.Setting();
        }
        else
        {
            shoulder_wrist.enabled = true;
            shoulder_wrist.Setting();
        }
        

        hasSet = true;
    }
    private void FixedUpdate()
    {
        GapHandler();
    }
    private void GapHandler()
    {
        if (hasTorque)
        {
            timer -= Time.deltaTime;
            if (timer < 0f)
            {
                float maxTimer = 0.5f;
                timer = maxTimer;
                float angle = transform.rotation.eulerAngles.x;
                angle = angle > 90 ? 360f - angle : -angle;
                gap = angle - pastAngle;
                updateGap++;
                pastAngle = angle;
            }
        }
    }
}
