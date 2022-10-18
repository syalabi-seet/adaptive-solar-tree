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
    [AddComponentMenu("ML Agents/Module Agent")]
    public class ModuleAgent : Agent
    {
        float m_latitude;
        float m_longitude;
        float m_elevation;
        double solarAltitude;
        double solarAzimuth;
        int startHour;
        int endHour;
        EnvironmentParameters m_ResetParams;
        GameObject lightSource;
        SunController sunController;
        LightSensorComponent[] lightSensorArray;
        ShadowRatioSensorComponent[] shadowRatioArray;
        ArticulationJointController jointController;
        ModuleController moduleController;
        BehaviorParameters behaviorParams;
        IncidenceAngleComponent incidenceAngleComponent;
        
        int numberOfLightSensors;
        int numberOfShadowRatioSensors;
        int numberOfSunStates = 2;
        int numberOfMotorStates = 5;
        int numberOfObservationStates;

        // Environment
        public SpawnManagerComponent spawnManager;
        public bool isController;
        public bool isRandomized = true;

        public float timeSpeed = 50f;
        public int frameSteps = 1;

        ModuleController.Joint[] joints;

        float incidenceAngle;

        public override void Initialize()
        {
            // Environment
            lightSource = GameObject.FindWithTag("LightSource");
            sunController = lightSource.GetComponent<SunController>();
            shadowRatioArray = gameObject.GetComponentsInChildren<ShadowRatioSensorComponent>();
            moduleController = gameObject.GetComponent<ModuleController>();
            joints = moduleController.joints;
            behaviorParams = gameObject.GetComponent<BehaviorParameters>();
            incidenceAngleComponent = gameObject.GetComponentInChildren<IncidenceAngleComponent>();

            m_latitude = transform.position.x;
            m_elevation = transform.position.y;
            m_longitude = transform.position.z;

            // Module
            numberOfShadowRatioSensors = shadowRatioArray.Length;

            int numberOfObservationStates = (
                numberOfLightSensors + 
                numberOfShadowRatioSensors + 
                numberOfSunStates +
                numberOfMotorStates);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Add shadow ratios per panel (6)
            foreach (ShadowRatioSensorComponent shadowRatioSensor in shadowRatioArray)
            {
                float shadowRatio = shadowRatioSensor.shadowRatio;
                sensor.AddObservation(shadowRatio);
            }
            
            // Add solar angles (2)
            DateTime m_time = sunController.time;
            float latitude = sunController.latitude + m_latitude;
            float longitude = sunController.longitude + m_longitude;
            SunPosition.CalculateSunPosition(
                m_time, (double)latitude, (double)longitude, 
                out solarAzimuth, out solarAltitude);
            sensor.AddObservation((float)solarAzimuth);
            sensor.AddObservation((float)solarAltitude);

            // Add current motor angle state (5)
            sensor.AddObservation(joints[0].robotPart.transform.eulerAngles.z);
            sensor.AddObservation(joints[1].robotPart.transform.rotation.y);
            sensor.AddObservation(joints[2].robotPart.transform.rotation.y);
            sensor.AddObservation(joints[3].robotPart.transform.rotation.z);
            sensor.AddObservation(joints[4].robotPart.transform.rotation.y);
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            // Speed up outside sun hours periods
            if (solarAltitude < 0f)
            {
                sunController.timeSpeed = timeSpeed * 10f;
            } else
            {
                sunController.timeSpeed = timeSpeed;
                // Move module
                var actions = actionBuffers.ContinuousActions;
                for (int i = 0; i < joints.Length; i++)
                {                     
                    jointController = joints[i].robotPart.GetComponent<ArticulationJointController>();                
                    jointController.RotateTo(actions[i]);             
                }     
            }          

            // Terminal states
            endHour = sunController.time.Hour;
            int deltaHour = endHour - startHour;
            if (deltaHour == 23)
            {
                EndEpisode();
            } 
        }

        public override void OnEpisodeBegin()
        {
            if (isController)
            {
                sunController.timeSpeed = timeSpeed;
                sunController.frameSteps = frameSteps;
                sunController.SetSun(isRandomized);
            }

            startHour = sunController.time.Hour;

            moduleController.ResetOrientation();
            
            spawnManager.SpawnObstacles();                  
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var continuousActionsOut = actionsOut.ContinuousActions;

            for (int i = 0; i < joints.Length; i++)
            {
                continuousActionsOut[i] = Input.GetAxis(joints[i].inputAxis);
            }
        }
    }
}