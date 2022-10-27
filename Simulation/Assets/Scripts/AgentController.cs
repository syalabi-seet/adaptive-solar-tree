using System;
using Random=UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

namespace Module
{
    [AddComponentMenu("ML Agents/Agent Controller")]
    public class AgentController : Agent
    {
        [SerializeField]
        public EnvironmentController environmentController;
        
        [SerializeField]
        public SunController sunController;

        [SerializeField]
        public SpawnManagerController spawnManager;

        ModuleController moduleController;
        ModuleController.Joint[] joints;
        GameObject[] solarPanels;

        float incidenceAngleLimit;
        float shadowRatioLimit;

        public override void Initialize()
        {
            moduleController = GetComponent<ModuleController>();
            joints = moduleController.joints;
            solarPanels = moduleController.solarPanels;
            incidenceAngleLimit = environmentController.incidenceAngleLimit;
            shadowRatioLimit = environmentController.shadowRatioLimit;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Add panel states (6)
            for (int i = 0; i < solarPanels.Length; i++)
            {
                // Add shadow ratio
                float shadowRatio = solarPanels[i].GetComponent<ShadowRatioSensorComponent>().shadowRatio;
                sensor.AddObservation(shadowRatio);
            }

            // Add location coordinates (2)
            sensor.AddObservation(
                Mathf.InverseLerp(-90f, 90f, (float)environmentController.latitude));
            sensor.AddObservation(
                Mathf.InverseLerp(-180f, 180f, (float)environmentController.longitude));  
            
            // Add solar angles (2)                
            sensor.AddObservation(
                Mathf.InverseLerp(0f, 360f, (float)sunController.solarAzimuth));
            sensor.AddObservation(
                Mathf.InverseLerp(-90f, 90f, (float)sunController.solarAltitude));

            // Add current motor angle state (5)
            for (int i = 0; i < joints.Length; i++)
            {
                float targetRotation = joints[i].robotPart.GetComponent<ArticulationBody>().xDrive.target;
                targetRotation = Mathf.InverseLerp(-180f, 180f, targetRotation);
                sensor.AddObservation(targetRotation);
            }         
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            int count = 0;
            for (int i = 0; i < solarPanels.Length; i++)
            {
                float shadowRatio = solarPanels[i].GetComponent<ShadowRatioSensorComponent>().shadowRatio;
                float incidenceAngle = solarPanels[i].GetComponent<IncidenceAngleComponent>().incidenceAngle;
        
                if (incidenceAngle < incidenceAngleLimit && shadowRatio < shadowRatioLimit)
                {
                    count++;
                }
            }

            if (count == solarPanels.Length)
            {
                EndEpisode();
            }
            else
            {
                SetReward(-0.001f);  
                moduleController.SetJoints(actionBuffers);           
            }
        }

        public override void OnEpisodeBegin()
        {
            environmentController.Reset();            
            spawnManager.SpawnObstacles();           
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var continuousActionsOut = actionsOut.ContinuousActions;
            continuousActionsOut[0] = Input.GetAxis("Joint1");
            continuousActionsOut[1] = Input.GetAxis("Joint2");
            continuousActionsOut[2] = Input.GetAxis("Joint3");
            continuousActionsOut[3] = Input.GetAxis("Joint4");
            continuousActionsOut[4] = Input.GetAxis("Joint5");         
        }
    }
}