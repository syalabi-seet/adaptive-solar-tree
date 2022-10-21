using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Module
{
    public enum trackingFrequencies
    {
        everySecond,
        everyMinute,
        everyHalfHour,
        everyHour,
        every2Hours,
        every4Hours,
        every24Hours
    }

    [AddComponentMenu("ML Agents/Environment Controller")]
    public class EnvironmentController : MonoBehaviour
    {
        [SerializeField]
        public bool isRandomized = true; 

        [SerializeField]
        public trackingFrequencies trackingFrequency = trackingFrequencies.everySecond; 

        [SerializeField]
        public GameObject lightSource;

        SunController sunController;     

        [SerializeField]
        public float timeScale = 50f;   

        [SerializeField]     
        public float shadowRatioLimit = 0.2f;
        
        [SerializeField]
        public float incidenceAngleLimit = 5f;        

        // Start is called before the first frame update
        void Start()
        {
            sunController = lightSource.GetComponent<SunController>();
        }

        public void ResetEnvironment()
        {
            sunController.timeScale = timeScale;
            sunController.SetSun(isRandomized);
        }
    }
}