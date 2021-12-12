using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CuriousAgent : Agent
{
    Rigidbody rBody;
    public float MovementForce;
    public Transform[] StartPositions;
    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
    }



    public Transform Target;
    public override void OnEpisodeBegin()
    {
            //Sets the position, rotation and velocity of the agent
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            this.transform.localPosition = StartPositions[Random.Range(0, StartPositions.Length)].localPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.InverseTransformDirection(rBody.velocity));
        sensor.AddObservation(this.transform.localPosition);
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
        }
        transform.Rotate(rotateDir, Time.deltaTime * 200f);
        rBody.AddForce(dirToGo * MovementForce, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)

    {
        AddReward(-1f / MaxStep);
        MoveAgent(actionBuffers.DiscreteActions);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Trap"))
        {
            SetReward(-1f);
            EndEpisode();
        }else if (collision.collider.CompareTag("Target"))
        {
            SetReward(2f);
            EndEpisode();
        }

    }

}

