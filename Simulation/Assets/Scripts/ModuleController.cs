using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

namespace Module
{
    public class ModuleController : Agent
    {
        private bool[] shadowStates;
        private int numberOfSensors;
        private float m_Elevation;
        ShadowSensor shadowSensor;
        EnvironmentParameters m_ResetParams; 

        private void Awake()
        {
            shadowSensor = GetComponent<ShadowSensor>();
            m_Elevation = transform.position.y;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Add shadow state to observation state (12)
            CollectShadowStates(sensor);

            // Add shadow ratios per module
            // CollectShadowRatios(sensor);
            
            // Add solar angles

            // Add current motor angle state (5)
        }

        private void CollectShadowStates(VectorSensor sensor)
        {
            shadowStates = shadowSensor.shadowStates;
            numberOfSensors = shadowSensor.numberOfSensors;
            for (int i = 0; i < numberOfSensors; i++)
            {
                sensor.AddObservation(Convert.ToInt32(shadowStates[i]));
            }
        }

        private void CollectShadowRatios(VectorSensor sensor)
        {

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
}