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
        float incidenceAngleLimit;
        float shadowRatioLimit;
        ModuleController moduleController;
        ModuleController.Joint[] joints;
        GameObject module;

        GameObject[] solarPanels;

        public override void Initialize()
        {
            module = GameObject.FindWithTag("Module");
            moduleController = gameObject.GetComponentInChildren<ModuleController>();
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
            sensor.AddObservation((float)environmentController.latitude);
            sensor.AddObservation((float)environmentController.longitude);
            
            // Add solar angles (2)    
            sensor.AddObservation((float)sunController.solarAzimuth);
            sensor.AddObservation((float)sunController.solarAltitude);

            // Add current motor angle state (5)
            for (int i = 0; i < joints.Length; i++)
            {
                float targetRotation = joints[i].robotPart.GetComponent<ArticulationBody>().xDrive.target;
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
                SetReward(1f);
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
            SetReward(0f);
            moduleController.ResetJoints();
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