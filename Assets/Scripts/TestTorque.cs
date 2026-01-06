using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class TestTorque : Agent
{
    [SerializeField] AddTorqueToGoal addTorqueinElbow;
    [SerializeField] SetGoalPosition setGoalPosition;
    [SerializeField] DetectTrigger detectTrigger;
    [SerializeField] SetGoalPosition setInitialPosition;

    public float manualInitialAngle = 0;
    public float manualTargetAngle = 90f;
    public float initialAngle = 0;
    public float targetAngle = 0;
    public float weight = 0;
    public float length = 0;
    public float multiplierTorque = 3000f;
    [SerializeField] private int myIndex;
    [SerializeField] private int myStep = 0;
    [SerializeField] private int myDecisionCount = 0;
    public bool hasTorque;
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
        if (!hasTorque && addTorqueinElbow.hasSet)
        {
            addTorqueinElbow.hasSet = false;
            hasTorque = true;
            addTorqueinElbow.Add(actions.ContinuousActions[0] * multiplierTorque);
        }
    }
    public float time = 10f;
    private float t;

    void Start()
    {
        t = time;
    }
    void Update()
    {
        time -= Time.deltaTime;
        if (time < 0)
        {
            time = t;
            //hasTorque = false;
            //GetComponent<ChangeParameter>().enabled = false;
            //EndEpisode();
        }
    }
}
