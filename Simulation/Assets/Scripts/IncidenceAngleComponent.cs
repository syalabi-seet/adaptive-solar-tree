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

        void Start()
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

            incidenceAngle = Vector3.Angle(direction, origin);

            Debug.DrawRay(origin, direction, Color.yellow);
        }
    }
}