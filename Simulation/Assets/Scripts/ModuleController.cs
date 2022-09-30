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
        public bool[] lightSensorStates;

        [HideInInspector]
        public List<GameObject> lightSensors;

        int numberOfSensors;
        float m_latitude;
        float m_longitude;
        float m_elevation;
        double solarAltitude;
        double solarAzimuth;
        private EnvironmentParameters m_ResetParams;

        [SerializeField]
        GameObject lightSource;

        private SunController sunController;

        public void Awake()
        {
            lightSource = GameObject.FindWithTag("LightSource"); 
            sunController = lightSource.GetComponent<SunController>();
            m_latitude = transform.position.x;
            m_elevation = transform.position.y;
            m_longitude = transform.position.z;   

            CollateLightSensors();      
        }

        void CollateLightSensors()
        {
            numberOfSensors = 0;
            GameObject[] children = GameObject.FindGameObjectsWithTag("LightSensor");
            foreach (GameObject child in children)
            {
                if (child.transform.parent == this.transform)
                {
                    lightSensors.Add(child);
                    numberOfSensors++;
                }
            }
            lightSensorStates = new bool[numberOfSensors]; 
        }

        public void Start()
        {
            
        }

        public void Update()
        {
            for (int i = 0; i < numberOfSensors; i++)
            {
                bool sensorState = lightSensors[i].GetComponent<LightSensor>().sensorState;
                lightSensorStates[i] = sensorState;
            }
        }

        public void FixedUpdate()
        {
            lightSensorStates = new bool[numberOfSensors];
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // // Add shadow state to observation state
            // lightSensorStates = lightSensorArray.sensorStates;
            // numberOfSensors = lightSensorArray.numberOfSensors;
            // for (int i = 0; i < numberOfSensors; i++)
            // {
            //     int shadowState = Convert.ToInt32(lightSensorStates[i]);
            //     sensor.AddObservation(shadowState);
            // }

            // Add shadow ratios per module (6)
            
            // // Add solar angles (2)
            // DateTime m_time = sunController.time;
            // float latitude = sunController.latitude + m_latitude;
            // float longitude = sunController.longitude + m_longitude;
            // SunPosition.CalculateSunPosition(
            //     m_time, (double)latitude, (double)longitude, out solarAzimuth, out solarAltitude);
            // sensor.AddObservation(latitude);
            // sensor.AddObservation(longitude);

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
}