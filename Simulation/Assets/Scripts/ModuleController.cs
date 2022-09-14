using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class ModuleController : Agent
{
    private bool[] shadowStates;
    private int numberOfSensors;
    ShadowSensor shadowSensor;
    EnvironmentParameters m_ResetParams; 

    private void Awake()
    {
        shadowSensor = GetComponent<ShadowSensor>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Add shadow state to observation state (12)
        shadowStates = shadowSensor.shadowStates;
        numberOfSensors = shadowSensor.numberOfSensors;
        for (int i = 0; i < numberOfSensors; i++)
        {
            sensor.AddObservation(Convert.ToInt32(shadowStates[i]));
        }        

        // Add target azimuth and zenith angle of module state (2)

        // Add current motor angle state (5)
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

    }

    public override void OnEpisodeBegin()
    {

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }
}
