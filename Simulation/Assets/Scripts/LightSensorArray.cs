using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    // [ExecuteInEditMode]
    [AddComponentMenu("ML Agents/Light Sensor Array")]
    public class LightSensorArray : MonoBehaviour
    {
        private int vectorLength = 5;
        public GameObject lightSource;
        public GameObject[] lightSensors;   
        public bool[] sensorStates;    

        void Awake()
        {
            lightSource = GameObject.FindWithTag("LightSource");
            lightSensors = GameObject.FindGameObjectsWithTag("LightSensor");
            sensorStates = new bool[lightSensors.Length];
        }

        void Update()
        {
            Vector3 direction = (
                transform.rotation *
                lightSource.transform.rotation *
                Vector3.back *
                vectorLength);

            int i = 0;
            foreach (GameObject lightSensor in lightSensors)
            {
                Vector3 origin = lightSensor.transform.position;

                // Cast ray
                bool rayOutput = Physics.Raycast(origin, direction);

                if (rayOutput)
                {
                    Debug.DrawRay(origin, direction, Color.red);
                    sensorStates[i] = true;
                } else
                {
                    Debug.DrawRay(origin, direction, Color.green);
                }
                i++;                    
            }
        }
    }
}