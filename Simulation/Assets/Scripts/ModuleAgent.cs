using System;
using Random=UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

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
        EnvironmentParameters m_ResetParams;
        GameObject lightSource;
        SunController sunController;
        LightSensorComponent[] lightSensorArray;
        ShadowRatioSensorComponent[] shadowRatioArray;
        
        int numberOfLightSensors;
        int numberOfShadowRatioSensors;
        int numberOfSunStates = 2;
        int numberOfMotorStates = 5;
        int numberOfObservationStates;

        ModuleController moduleController;
        ModuleController.Joint[] joints;

        // Environment
        public SpawnManager spawnManager;
        public bool isController;
        public bool isRandomized = true;

        public float timeSpeed = 50f;
        public int frameSteps = 1;

        public override void Initialize()
        {
            // Environment
            lightSource = GameObject.FindWithTag("LightSource");
            sunController = lightSource.GetComponent<SunController>();
            lightSensorArray = gameObject.GetComponentsInChildren<LightSensorComponent>();
            shadowRatioArray = gameObject.GetComponentsInChildren<ShadowRatioSensorComponent>();

            m_latitude = transform.position.x;
            m_elevation = transform.position.y;
            m_longitude = transform.position.z;

            // Module
            numberOfLightSensors = lightSensorArray.Length;
            numberOfShadowRatioSensors = shadowRatioArray.Length;

            int numberOfObservationStates = (
                numberOfLightSensors + 
                numberOfShadowRatioSensors + 
                numberOfSunStates +
                numberOfMotorStates);          

            moduleController = gameObject.GetComponent<ModuleController>();
            joints = moduleController.joints;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Add shadow state to observation state (12)
            for (int i = 0; i < numberOfLightSensors; i++)
            {
                float sensorState = Convert.ToSingle(lightSensorArray[i].sensorState);
                sensor.AddObservation(sensorState);
            }

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
            var actions = actionBuffers.ContinuousActions;

            // TODO: NEED TO MAKE IT DISCRETE ACTIONS WITH INCREMENTAL ANGLES
            for (int i = 0; i < joints.Length; i++)
            {
                float inputVal = actions[i];
                if (Mathf.Abs(inputVal) > 0)
                {
                    RotationDirection direction = GetRotationDirection(inputVal);
                    moduleController.RotateJoint(i, direction);
                }
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

        static RotationDirection GetRotationDirection(float inputVal)
        {
            if (inputVal > 0)
            {
                return RotationDirection.Positive;
            }
            else if (inputVal < 0)
            {
                return RotationDirection.Negative;
            }
            else
            {
                return RotationDirection.None;
            }
        }
    }
}