using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    [AddComponentMenu("ML Agents/Light Sensor")]
    public class LightSensor : MonoBehaviour
    {
        private int vectorLength = 5;
        public GameObject lightSource;
        public bool sensorState;

        void Update()
        {
            Vector3 direction = (
                transform.rotation *
                lightSource.transform.rotation *
                Vector3.back *
                vectorLength);

            Vector3 origin = transform.position;

            RaycastHit[] hits = Physics.RaycastAll(origin, direction, Mathf.Infinity);

            bool rayOutput = FilterTags(hits);

            if (rayOutput && direction.y > 0)
            {
                Debug.DrawRay(origin, direction, Color.red);
                sensorState = true;
            } else
            {
                Debug.DrawRay(origin, direction, Color.green);
                sensorState = false;
            }    
        }

        void FixedUpdate()
        {
            sensorState = false;
        }

        bool FilterTags(RaycastHit[] hits)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.tag == "Obstacle")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
