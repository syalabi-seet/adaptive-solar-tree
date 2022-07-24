using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDetection : MonoBehaviour
{
    public bool drawRays = true;
    public bool[] shadowStates;
    private int numRays = 4;    
    private int vectorLength = 100;
    private float rotationAngle;

    private GameObject lightSource;

    // Start is called before the first frame update
    private void Start()
    {
        lightSource = GameObject.FindWithTag("LightSource");
        shadowStates = new bool[numRays];
    }

    // Update is called once per frame
    private void Update()
    {
        if (drawRays)
        {
            DetectRays();   
        }        
    }

    private void FixedUpdate()
    {
        shadowStates = new bool[numRays];
    }

    private void DetectRays()
    {
        rotationAngle = (Mathf.PI * 2) / numRays;
        Vector3 forward = transform.rotation * lightSource.transform.rotation * Vector3.back * vectorLength;
        for (int i = 0; i < numRays; i++)
        {
            Vector3 origin = GetOrigin(i);
            Debug.DrawRay(origin, forward, Color.green);
            if (Physics.Raycast(origin, forward) && forward.y > 0)
            {
                Debug.DrawRay(origin, forward, Color.red);
                shadowStates[i] = true;
            }
        }
    }

    private Vector3 GetOrigin(int i)
    {
        float y = transform.localScale.y;
        float radius = transform.localScale.x / 2.0f;
        float x = radius * Mathf.Cos(i * rotationAngle);
        float z = radius * Mathf.Sin(i * rotationAngle);
        Vector3 origin = transform.rotation * (transform.position + new Vector3(x, y, z));
        return origin;
    }    
}
