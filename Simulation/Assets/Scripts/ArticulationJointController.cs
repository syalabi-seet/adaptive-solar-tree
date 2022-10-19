using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    [AddComponentMenu("ML Agents/Articulation Joint Controller")]
    public class ArticulationJointController : MonoBehaviour
    {
        ArticulationBody articulationBody;
        public float forceMultiplier;
        public float rotationChange;   

        void Start()
        {
            articulationBody = GetComponent<ArticulationBody>();
        }

        float CurrentPrimaryRotation()
        {
            float currentRotationRads = articulationBody.jointPosition[0];
            float currentRotation = Mathf.Rad2Deg * currentRotationRads;
            return currentRotation;
        }

        public void RotateTo(float inputVal)
        {
            float currentRotation = CurrentPrimaryRotation();
            var drive = articulationBody.xDrive;
            rotationChange = inputVal;
            float targetRotation = Mathf.Round(currentRotation + rotationChange);
            drive.target = targetRotation;       
            articulationBody.xDrive = drive;       
        }

        public void Reset()
        {
            var drive = articulationBody.xDrive;
            drive.target = 0f;
            articulationBody.xDrive = drive; 
        }
    }
}
