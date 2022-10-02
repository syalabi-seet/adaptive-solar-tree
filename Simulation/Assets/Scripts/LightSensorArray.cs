using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    [AddComponentMenu("ML Agents/Light Sensor Array Component")]
    public class LightSensorArray : MonoBehaviour
    {
        private LightSensorComponent[] lightSensors;
        public bool[] lightSensorStates;
        int numberOfSensors;

        void Awake()
            {
                lightSensors = gameObject.GetComponentsInChildren<LightSensorComponent>();
                numberOfSensors = lightSensors.Length;
            }

        public void Update()
        {
            for (int i = 0; i < numberOfSensors; i++)
            {
                lightSensorStates[i] = lightSensors[i].sensorState;
            }
        }

        public void FixedUpdate()
        {
            lightSensorStates = new bool[numberOfSensors];
        }
    }
}