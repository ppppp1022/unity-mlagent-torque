using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeParameterWrist_Env : MonoBehaviour
{
    public ArticulationBody shoulder;
    public ArticulationBody elbow;
    public ArticulationBody wrist;

    private float length;
    private float shoulderLength;
    private float elbowLength;
    private float wristLength;

    private float weight;
    private float shoulderWeight;
    private float elbowWeight;
    private float wristWeight;

    [SerializeField] private float shoulderAngle;
    [SerializeField] private float elbowAngle;
    [SerializeField] private float wristAngle;
    
    [SerializeField] private Vector3 shoulderAngularVelocity;
    [SerializeField] private Vector3 elbowAngularVelocity;
    [SerializeField] private Vector3 wristAngularVelocity;
    
    private float past_t_gravity;
    private float past_elbow_angle = 0;
    private float past_wrist_angle = 0;
    private float timer = 0.05f;
    public void Setting()
    {
        //Debug.Log("Awake");
        weight = GetComponent<ManageWristArticulationBody>().weight;
        length = GetComponent<ManageWristArticulationBody>().length;
        past_elbow_angle = GetComponent<ManageWristArticulationBody>().initialAngle;
        shoulderLength = length * 1 / 2 * 0.46f;
        elbowLength = length * 1 / 2 * 0.3f;
        wristLength = length * 1 / 2 * 0.24f;
        shoulderWeight = weight * 0.027f;
        shoulder.mass = shoulderWeight;
        elbowWeight = weight * 0.016f;
        elbow.mass = elbowWeight;
        wristWeight = weight * 0.0065f;
        wrist.mass = wristWeight;
        /*
        shoulder.transform.localScale = new Vector3(shoulder.transform.localScale.x, shoulderLength, shoulder.transform.localScale.z);
        elbow.transform.localScale = new Vector3(shoulder.transform.localScale.x, elbowLength, shoulder.transform.localScale.z);
        wrist.transform.localScale = new Vector3(shoulder.transform.localScale.x, wristLength, shoulder.transform.localScale.z);
        elbow.transform.position = shoulder.transform.position + new Vector3(0, - shoulderLength-elbowLength + 1 / 3f, 0);
        wrist.transform.position = elbow.transform.position + new Vector3(0, - elbowLength - wristLength + 1 / 3f, 0);

        elbow.transform.SetParent(shoulder.transform);
        wrist.transform.SetParent(elbow.transform);
        */
    }
    void Update()
    {
        if (wrist.xDrive.driveType == ArticulationDriveType.Force)
        {
            UpdateParameter();
        }
    }
    public void UpdateParameter()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            past_wrist_angle = wristAngle;
            wristAngle = wrist.transform.rotation.eulerAngles.z;
            wristAngle = wristAngle > 90 ? 360f - wristAngle : -wristAngle;

            double I_1 = 1f / 3f * shoulderWeight * shoulderLength;
            double I_2 = 1f / 3f * elbowWeight * elbowLength;
            double I_3 = 1f / 3f * wristWeight * wristLength;
            double I_eq = I_1 + (I_2 + I_3) * Mathf.Pow(shoulderLength / elbowLength, 2);

            float t_gravity = shoulderWeight * 9.8f * Mathf.Cos(shoulderAngle) + elbowWeight * 9.8f * Mathf.Cos(elbowAngle) +
                    wristWeight * 9.8f * Mathf.Cos(wristAngle);

            float K_passive = 2.0f + 1.5f * Mathf.Cos(wristAngle);
            float K_muscle = (float)I_eq * (Mathf.Abs(wristAngle) / 30 + 2) * 10;
            float K_gravity = 0;
            if (wristAngle - past_wrist_angle <= 0.1f)
            {
                K_gravity = 0;
            }
            else
            {
                K_gravity = Mathf.Abs((t_gravity - past_t_gravity) / (wristAngle - past_wrist_angle));
            }
            float K_wrist = K_passive + K_muscle + K_gravity;

            float B_viscous = 0.15f * (float)I_eq + 0.5f;
            float B_muscle = 0.5f * (Mathf.Abs(wristAngle) / 30 + 1) * 10 * Mathf.Sqrt((float)I_eq);
            float B_angle = 0.1f * Mathf.Abs(Mathf.Sin(wristAngle));
            float B_wrist = B_viscous + B_muscle + B_angle;

            var xDrive = wrist.xDrive;
            xDrive.target = wristAngle;
            xDrive.damping = B_wrist*2f;
            xDrive.stiffness = K_wrist;
            wrist.xDrive = xDrive;
            timer = 0.01f;
        }
        else
        {
            float currentAngle = wrist.transform.rotation.eulerAngles.z;
            currentAngle = currentAngle > 90 ? 360f - currentAngle : -currentAngle;
            var xDrive = wrist.xDrive;
            xDrive.target = currentAngle;
            wrist.xDrive = xDrive;
        }
    }
}

