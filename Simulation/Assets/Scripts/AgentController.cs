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
        float m_latitude;
        float m_longitude;
        float m_elevation;
        double solarAltitude;
        double solarAzimuth;
        int startHour;
        int endHour;
        int currentDeltaHour;
        EnvironmentController environmentController;
        
        [SerializeField]
        public GameObject lightSource;

        SunController sunController;
        ModuleController moduleController;
        SpawnManagerController spawnManagerController;
        IncidenceAngleComponent incidenceAngleComponent;
        GameObject[] joints;
        GameObject[] solarPanels;

        public GameObject spawnManager;
        float timeScale; 

        public override void Initialize()
        {
            // Environment
            sunController = lightSource.GetComponent<SunController>();
            moduleController = gameObject.GetComponent<ModuleController>();
            spawnManagerController = spawnManager.GetComponent<SpawnManagerController>();
            environmentController = GameObject.FindWithTag("GameController").GetComponent<EnvironmentController>();
            joints = moduleController.joints;
            solarPanels = moduleController.solarPanels;

            m_latitude = transform.position.x;
            m_elevation = transform.position.y;
            m_longitude = transform.position.z;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Add panel states (12)
            foreach (GameObject solarPanel in solarPanels)
            {
                // Add shadow ratio
                float shadowRatio = solarPanel.GetComponent<ShadowRatioSensorComponent>().shadowRatio;
                sensor.AddObservation(shadowRatio);
                
                // Add incidence angle
                float incidenceAngle = solarPanel.GetComponent<IncidenceAngleComponent>().incidenceAngle;
                sensor.AddObservation(incidenceAngle);
            }
            
            // Add solar angles (2)
            DateTime m_time = sunController.time;
            float latitude = sunController.latitude + m_latitude;
            float longitude = sunController.longitude + m_longitude;
            SunPosition.CalculateSunPosition(
                m_time, (double)latitude, (double)longitude, 
                out solarAzimuth, out solarAltitude);
            sensor.AddObservation(
                Mathf.InverseLerp(-180f, 180f, (float)solarAzimuth));
            sensor.AddObservation(
                Mathf.InverseLerp(0f, 360f, (float)solarAltitude));

            // TODO: Add current motor angle state (5)
            foreach (GameObject joint in joints)
            {
                float targetRotation = joint.GetComponent<ArticulationBody>().xDrive.target;
                sensor.AddObservation(targetRotation);
            }         
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            // Terminal state
            endHour = sunController.time.Hour;
            int deltaHour = endHour - startHour;
            if (deltaHour == 23)
            {
                EndEpisode();
            }

            // Speed up outside sun hours
            if (solarAltitude < 0f)
            {
                sunController.timeScale = 100000f;
            } else 
            {
                sunController.timeScale = environmentController.timeScale;
                moduleController.SetOrientation(actionBuffers);         
            }
        }

        public override void OnEpisodeBegin()
        {
            spawnManagerController.DestroyObstacles();
            environmentController.ResetEnvironment();
            startHour = sunController.time.Hour;
            moduleController.ResetOrientation();            
            spawnManagerController.SpawnObstacles();                  
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