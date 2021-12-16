using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class CuriousAgent2 : Agent
{
    Rigidbody rBody;
    public float MovementForce;
    public Transform[] StartPositions;
    public GameObject PrefabTarget;
    public GameObject PrefabTrap;
    public GameObject[] TrapOrTargetPlaceholders;
    private List<GameObject> Traps = new List<GameObject>();
    private List<GameObject> Targets = new List<GameObject>();
    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        //Sets the position, rotation and velocity of the agent
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
        this.transform.localPosition = StartPositions[Random.Range(0, StartPositions.Length)].localPosition;

        //Creates randomly the traps and the target with the placeholders
        if (Traps.Count > 0)
        {
            foreach (GameObject trap in Traps)
            {
                Destroy(trap);
            }
        }
        if (Targets.Count > 0)
        {
            foreach (GameObject target in Targets)
            {
                Destroy(target);
            }
        }

        Traps.Clear();
        Targets.Clear();


        for (int i = 0; i < TrapOrTargetPlaceholders.Length - 1; i++)
        {
            int rnd = Random.Range(i, TrapOrTargetPlaceholders.Length);
            var tempGO = TrapOrTargetPlaceholders[rnd];
            TrapOrTargetPlaceholders[rnd] = TrapOrTargetPlaceholders[i];
            TrapOrTargetPlaceholders[i] = tempGO;
        }

        //The first element will be the target
        Targets.Add(Instantiate(PrefabTarget, TrapOrTargetPlaceholders[0].transform.position, Quaternion.identity, TrapOrTargetPlaceholders[0].transform));

        //Instantiate the rest of the elements as traps
        for (var i = 1; i < TrapOrTargetPlaceholders.Length; i++)
        {
            Traps.Add(Instantiate(PrefabTrap, TrapOrTargetPlaceholders[i].transform.position, Quaternion.identity, TrapOrTargetPlaceholders[i].transform));
        }

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

