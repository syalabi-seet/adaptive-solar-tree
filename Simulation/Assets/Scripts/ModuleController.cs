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
        }

        public void Start()
        {
            float positionRange = 1.0f;
            float rotationRange = 30.0f;

            Vector3 originalPosition = gameObject.transform.position;

            Vector3 newPosition = originalPosition + new Vector3(
                Random.Range(-positionRange, positionRange), 
                Random.Range(0.0f, 1.0f),
                Random.Range(-positionRange, positionRange));

            gameObject.transform.position = newPosition;

            Quaternion newRotation = Quaternion.Euler(
                Random.Range(-rotationRange, rotationRange),
                Random.Range(-rotationRange, rotationRange),
                Random.Range(-rotationRange, rotationRange));

            gameObject.transform.rotation = newRotation;
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
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