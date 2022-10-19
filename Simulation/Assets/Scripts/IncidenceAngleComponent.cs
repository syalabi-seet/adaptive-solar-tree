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
        ModuleAgent moduleAgent;

        void Start()
        {
            shadowRatioSensor = gameObject.GetComponent<ShadowRatioSensorComponent>();
            moduleAgent = gameObject.GetComponentInParent<ModuleAgent>();
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
            float shadowRatio = shadowRatioSensor.shadowRatio;
            float incidenceAngleLimit = moduleAgent.incidenceAngleLimit;
            float shadowRatioLimit = moduleAgent.shadowRatioLimit;

            if (direction.y > 0)
            {
                if (incidenceAngle < incidenceAngleLimit && shadowRatio < shadowRatioLimit)
                {
                    Debug.DrawRay(origin, direction, Color.green);
                } else if (shadowRatio < shadowRatioLimit)
                {
                    Debug.DrawRay(origin, direction, Color.yellow);
                } else
                {
                    Debug.DrawRay(origin, direction, Color.red);
                }
            }     
        }
    }
}