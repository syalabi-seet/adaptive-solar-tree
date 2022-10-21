using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class IncidenceAngleComponent : MonoBehaviour
    {
        private int vectorLength = 6;
        public GameObject lightSource;
        public float incidenceAngle;
        ShadowRatioSensorComponent shadowRatioSensor;
        EnvironmentController environmentController;
        AgentController agentController;
        float incidenceAngleLimit;
        float shadowRatioLimit;
        float shadowRatio;

        void Start()
        {
            shadowRatioSensor = gameObject.GetComponent<ShadowRatioSensorComponent>();
            agentController = gameObject.GetComponentInParent<AgentController>();
            environmentController = GameObject.FindWithTag("GameController").GetComponent<EnvironmentController>();

            incidenceAngleLimit = environmentController.incidenceAngleLimit;
            shadowRatioLimit = environmentController.shadowRatioLimit;
            shadowRatio = shadowRatioSensor.shadowRatio;
        }

        void Update()
        {
            Vector3 direction = (
                transform.localRotation *
                lightSource.transform.localRotation *
                Vector3.back *
                vectorLength);

            Vector3 origin = transform.position;
            Vector3 normal = transform.up * vectorLength;
            incidenceAngle = Vector3.Angle(normal, direction);

            if (direction.y > 0)
            {
                if (incidenceAngle < incidenceAngleLimit && shadowRatio < shadowRatioLimit)
                {
                    Debug.DrawRay(origin, direction, Color.green);    
                    agentController.AddReward(0.01f);           
                } else if (shadowRatio < shadowRatioLimit)
                {
                    Debug.DrawRay(origin, direction, Color.yellow);
                } else
                {
                    Debug.DrawRay(origin, direction, Color.red);
                }
            } else
            {
                Debug.DrawRay(origin, direction, Color.red);
            }
        }
    }
}