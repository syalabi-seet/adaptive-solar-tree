using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class IncidenceAngleComponent : MonoBehaviour
    {
        [SerializeField]
        public GameObject lightSource;
        public float incidenceAngle;

        private int vectorLength = 6;
        ShadowRatioSensorComponent shadowRatioSensor;
        EnvironmentController environmentController;
        AgentController agentController;
        float incidenceAngleLimit;
        float shadowRatioLimit;
        float shadowRatio;

        Vector3 direction;

        Vector3 origin;
        Vector3 normal;

        Color color;

        void Start()
        {
            shadowRatioSensor = gameObject.GetComponent<ShadowRatioSensorComponent>();
            agentController = gameObject.GetComponentInParent<AgentController>();
            environmentController = GameObject.FindWithTag("GameController").GetComponent<EnvironmentController>();

            incidenceAngleLimit = environmentController.incidenceAngleLimit;
            shadowRatioLimit = environmentController.shadowRatioLimit;
        }

        void Update()
        {
            direction = (
                lightSource.transform.localRotation *
                Vector3.back *
                vectorLength);

            origin = transform.position;
            normal = transform.up * vectorLength;
            incidenceAngle = Vector3.Angle(normal, direction);
            shadowRatio = shadowRatioSensor.shadowRatio;

            if (direction.y > 0)
            {
                if (incidenceAngle < incidenceAngleLimit && shadowRatio < shadowRatioLimit)
                {
                    color = Color.green;
                } 
                else if (incidenceAngle < incidenceAngleLimit || shadowRatio < shadowRatioLimit)
                {
                    color = Color.yellow;
                } 
                else
                {
                    color = Color.red;
                }
            } else
            {
                color = Color.red; 
            }
            Debug.DrawRay(origin, direction, color);
        }
    }
}