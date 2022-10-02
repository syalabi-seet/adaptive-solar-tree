using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    [AddComponentMenu("ML Agents/Light Sensor Component")]
    public class LightSensorComponent : MonoBehaviour
    {
        private int vectorLength = 6;
        public GameObject lightSource;
        public bool sensorState;

        void Awake()
        {
            lightSource = GameObject.FindWithTag("LightSource");
        }

        void Update()
        {
            Vector3 direction = (
                transform.localRotation *
                lightSource.transform.rotation *
                Vector3.back *
                vectorLength);

            Vector3 origin = transform.position;

            RaycastHit[] hits = Physics.RaycastAll(origin, direction, Mathf.Infinity);

            bool rayOutput = FilterTags(hits);

            float altitude = direction.y;

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
                string tag = hit.transform.tag;
                if (tag == "Obstacle" || tag == "Module")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
