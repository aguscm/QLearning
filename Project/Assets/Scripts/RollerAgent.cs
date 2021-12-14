using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RollerAgent : Agent
{
    Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Target;
    public Transform Trap;
    public override void OnEpisodeBegin()
    {
        // If the Agent fell, zero its momentum
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        // Move the target to a new spot far (>2f) from the ball
        bool TargetIsPositioned = false;
        while (!TargetIsPositioned)
        {

            Vector3 TargetPosition = new Vector3(Random.value * 8 - 4,
                                           0.5f,
                                           Random.value * 8 - 4);
            if ((TargetPosition - this.gameObject.transform.localPosition).magnitude > 2f)
            {
                Target.localPosition = TargetPosition;
                TargetIsPositioned = true;
            }
        }
        

        // Move the forbidden target to a new spot far (>3f) from the target and far (2f) from the ball
        if (Trap)
        {
            bool TrapIsPositioned = false;
            while (!TrapIsPositioned)
            {
                Vector3 TrapPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
                if ((TrapPosition - Target.localPosition).magnitude > 3f && (TrapPosition - this.gameObject.transform.localPosition).magnitude > 2f)
                {
                    Trap.localPosition = TrapPosition;
                    TrapIsPositioned = true;
                }
            }
        }

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(Trap.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);


        // Fell off platform
        if (this.transform.localPosition.y < 0)
        {
            SetReward(-0.5f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Trap"))
        {
            SetReward(-0.5f);
            EndEpisode();
        }
        else if (collision.collider.CompareTag("Target"))
        {
            SetReward(1f);
            EndEpisode();
        }

    }

}
