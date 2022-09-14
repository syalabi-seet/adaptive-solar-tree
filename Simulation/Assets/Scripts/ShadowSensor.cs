using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
[AddComponentMenu("ML Agents/Shadow Sensor")]
public class ShadowSensor : MonoBehaviour
{
    public bool drawRays = true;
    public bool[] shadowStates;
    public int numberOfSensors = 12;    
    public int vectorLength = 100;
    private float rotationAngle;

    private GameObject lightSource;

    private void Awake()
    {
        lightSource = GameObject.FindWithTag("LightSource");
        shadowStates = new bool[numberOfSensors];
        rotationAngle = (Mathf.PI * 2) / numberOfSensors;
    }

    private void Update()
    {
        Vector3 direction = (
            transform.rotation * lightSource.transform.rotation * 
            Vector3.back * vectorLength);
        for (int i = 0; i < numberOfSensors; i++)
        {
            Vector3 origin = GetOrigin(i);
            bool rayOutput = Physics.Raycast(origin, direction);

            // Update shadow states of each sensor
            if (rayOutput && direction.y > 0)
            {
                shadowStates[i] = true;
            }

            if (drawRays)
            {
                // Change colour of raycast depending on collision state
                if (rayOutput && direction.y > 0)
                {
                    Debug.DrawRay(origin, direction, Color.red); 
                } else
                {
                    Debug.DrawRay(origin, direction, Color.green); 
                }
            }
        }
    }

    private void FixedUpdate()
    {
        shadowStates = new bool[numberOfSensors];
    }

    private Vector3 GetOrigin(int i)
    {
        float y = transform.localScale.y;
        float radius = transform.localScale.x / 2.0f;
        float x = radius * Mathf.Cos(i * rotationAngle);
        float z = radius * Mathf.Sin(i * rotationAngle);
        Vector3 origin = (
            transform.rotation * (transform.position + new Vector3(x, y, z)));
        return origin;
    }
}