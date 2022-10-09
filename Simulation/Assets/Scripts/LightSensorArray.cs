using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    [AddComponentMenu("ML Agents/Light Sensor Array Component")]
    public class LightSensorArray : MonoBehaviour
    {
        private LightSensorComponent[] lightSensors;
        public bool[] sensorStates;

        [HideInInspector]
        public int numberOfSensors;

        void Awake()
            {
                lightSensors = gameObject.GetComponentsInChildren<LightSensorComponent>();
                numberOfSensors = lightSensors.Length;
            }

        public void Update()
        {
            for (int i = 0; i < numberOfSensors; i++)
            {
                sensorStates[i] = lightSensors[i].sensorState;
            }
        }

        public void FixedUpdate()
        {
            sensorStates = new bool[numberOfSensors];
        }
    }
}