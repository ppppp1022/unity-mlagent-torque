using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class ManageArticulationBody : Agent
{
    [SerializeField] AddTorqueToGoal addTorqueinElbow;
    [SerializeField] SetGoalPosition setGoalPosition;
    [SerializeField] CheckPerformence supervisor;
    [SerializeField] DetectTrigger detectTrigger;
    [SerializeField] SetGoalPosition setInitialPosition;

    [SerializeField] Material winMaterial;
    [SerializeField] Material drawMaterial;
    [SerializeField] Material lossMaterial;
    [SerializeField] MeshRenderer floorMesh;

    public float manualInitialAngle = 0;
    public float manualTargetAngle = 90f;
    public float initialAngle = 0;
    public float targetAngle = 0;
    public float weight = 0;
    public float length = 0;
    public float multiplierTorque = 3000f;
    [SerializeField] private int myIndex;
    [SerializeField] private int myStep=0;
    [SerializeField] private int myDecisionCount = 0;
    private bool hasTorque;
    private float myTorque;

    public override void OnEpisodeBegin()
    {
        //Debug.Log("begin");
        //초기 값 세팅
        myStep++;
        detectTrigger.outOfRange = false;
        //initialAngle = Random.Range(0f, 90);
        initialAngle = manualInitialAngle;
        //targetAngle = Random.Range(0f, 90);
        targetAngle = manualTargetAngle;
        weight = Random.Range(65f, 80f);
        length = Random.Range(0.165f, 0.180f);
        hasTorque = false;
        //다른 컴포넌트 값 초기화
        addTorqueinElbow.ResetVariable();

        setGoalPosition.SetPosition(targetAngle);
        setInitialPosition.SetPosition(initialAngle);
        StartCoroutine(addTorqueinElbow.SetPosition(initialAngle));
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(targetAngle);
        sensor.AddObservation(initialAngle);
        sensor.AddObservation(weight);
        sensor.AddObservation(length);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        myDecisionCount++;
        float angle = addTorqueinElbow.transform.rotation.eulerAngles.x;
        angle = angle > 90 ? 360f - angle : -angle;
        if (!hasTorque && addTorqueinElbow.hasSet)
        {
            hasTorque = true;
            addTorqueinElbow.hasSet = false;
            myTorque = actions.ContinuousActions[0];
            //Debug.Log($"index: {myIndex}, torque: {myTorque*3000}");
            StartCoroutine(Action(myTorque * multiplierTorque, myStep));
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        if (!hasTorque)
        {
            ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
            //continuousActions[0] = Random.Range(-1f, 1f);
        }
    }
    IEnumerator Action(float torque, float mystep)
    {
        addTorqueinElbow.Add(torque);
        yield return new WaitForSeconds(20f);
        //yield return new WaitUntil(() => addTorqueinElbow.updateGap > 4);
        //yield return new WaitUntil(() => Mathf.Abs(addTorqueinElbow.gap) < 1f);

        float currentAngle = addTorqueinElbow.transform.rotation.eulerAngles.x;
        currentAngle = currentAngle > 90 ? 360f - currentAngle : -currentAngle;
        float angleDifference = Mathf.Abs(targetAngle - currentAngle);

        float penalty = detectTrigger.outOfRange ? -(1+Mathf.Abs(myTorque))/2f : 1f;
        // 3. 방향 보상 (단순화)
        float previousDifference = Mathf.Abs(targetAngle - initialAngle);
        float currentDifference = Mathf.Abs(targetAngle - currentAngle);
        float directionReward = (previousDifference > currentDifference) ? 1f : -(1+Mathf.Abs(myTorque))/2f;

        float gapReward = 2 * Mathf.Exp(-Mathf.Sqrt(Mathf.Abs((Mathf.Abs(targetAngle) - Mathf.Abs(currentAngle)) / 28))) - 1f;

        float finalReward = gapReward * 0.5f + penalty * 0.3f + directionReward * 0.2f; //e1
        //float finalReward = gapReward * 0.4f + penalty * 0.3f + directionReward * 0.4f; //e2
        //float finalReward = gapReward * 0.6f + penalty * 0.3f + directionReward * 0.1f; //e3
        //float finalReward = gapReward * 0.9f + penalty * 0.1f + directionReward * 0.05f; //e4
        //Debug.Log($"{myIndex}, {mystep}:: {torque}, finalReward: {finalReward}, gapReward: {gapReward}, penalty: {penalty}, directionReward: {directionReward}");
        SetReward(finalReward);

        if (angleDifference < 10)
        {
            AddReward(0.1f * gapReward);
            floorMesh.material = winMaterial;
            supervisor.UpdateCount(0, myIndex);
        }
        else if (angleDifference < 20f)
        {
            AddReward(0.05f * gapReward);
            floorMesh.material = drawMaterial;
            supervisor.UpdateCount(1, myIndex);
        }
        else
        {
            floorMesh.material = lossMaterial;
            supervisor.UpdateCount(2, myIndex);
        }

        //Debug.Log($"End finalReward: {finalReward}");
        GetComponent<ChangeParameter>().enabled = false;
        //Debug.Log("end");
        EndEpisode();
    }
}
