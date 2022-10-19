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
    public enum trackingFrequencies 
    {
        Minutely = 0,
        Halfhourly = 1,
        Bihourly = 2,
        Quadhourly = 3,
        Daily = 4
    };

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

        [Range(0, 4)]
        public int trackingFrequency;

        public float timeSpeed = 50f;
        public int frameSteps = 1;

        public float shadowRatioLimit = 0.2f;
        public float incidenceAngleLimit = 5f;        

        [System.Serializable]
        public struct Joint
        {
            public string inputAxis;
            public GameObject robotPart;
            public float forceMultiplier;
        }
        public Joint[] joints;

        float incidenceAngle;

        public override void Initialize()
        {
            // Environment
            lightSource = GameObject.FindWithTag("LightSource");
            sunController = lightSource.GetComponent<SunController>();
            shadowRatioArray = gameObject.GetComponentsInChildren<ShadowRatioSensorComponent>();
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

        // void Update()
        // {
        //     // TODO: Adjust request decision frequency
        //     RequestDecision();
        // }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            // Speed up outside sun hours periods
            if (solarAltitude < 0f)
            {
                sunController.timeSpeed = timeSpeed * 10000f;
            } else
            {
                sunController.timeSpeed = timeSpeed;
                // Move module
                var actions = actionBuffers.DiscreteActions;
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

            // TODO: Add end episode when collision occurs
        }

        void OnCollisionEnter(Collision collision)
        {
            print(collision.gameObject);
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

            ResetOrientation();
            
            spawnManager.SpawnObstacles();                  
        }

        void ResetOrientation()
        {
            for (int i = 0; i < joints.Length; i++)
            {
                jointController = joints[i].robotPart.GetComponent<ArticulationJointController>();
                jointController.forceMultiplier = joints[i].forceMultiplier;
                jointController.Reset();            
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteActionsOut = actionsOut.DiscreteActions;

            // Joint1
            if (Input.GetKey(KeyCode.Q))
            {
                discreteActionsOut[0] = 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                discreteActionsOut[0] = -1;
            }

            // Joint2
            if (Input.GetKey(KeyCode.W))
            {
                discreteActionsOut[1] = 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                discreteActionsOut[1] = -1;
            }
            
            // Joint3
            if (Input.GetKey(KeyCode.E))
            {
                discreteActionsOut[2] = 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                discreteActionsOut[2] = -1;
            }

            // Joint4
            if (Input.GetKey(KeyCode.R))
            {
                discreteActionsOut[3] = 1;
            }
            if (Input.GetKey(KeyCode.F))
            {
                discreteActionsOut[3] = -1;
            }

            // Joint5
            if (Input.GetKey(KeyCode.T))
            {
                discreteActionsOut[4] = 1;
            }
            if (Input.GetKey(KeyCode.G))
            {
                discreteActionsOut[4] = -1;
            }
        }
    }
}