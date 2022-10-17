using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public enum RotationDirection {None = 0, Positive = 1, Negative = -1};

    [AddComponentMenu("ML Agents/Articulation Joint Controller")]
    public class ArticulationJointController : MonoBehaviour
    {
        public RotationDirection rotationState = RotationDirection.None;
        private float timeSpeed = 50f;
        private ArticulationBody articulationBody;

        void Start()
        {
            articulationBody = GetComponent<ArticulationBody>();
        }

        void FixedUpdate()
        {
            if (rotationState != RotationDirection.None)
            {
                float rotationChange = (float)rotationState * timeSpeed * Time.fixedDeltaTime;
                float rotationGoal = CurrentPrimaryAxisRotation() + rotationChange;
                RotateTo(rotationGoal);
            }
        }

        float CurrentPrimaryAxisRotation()
        {
            float currentRotationRads = articulationBody.jointPosition[0];
            float currentRotation = Mathf.Rad2Deg * currentRotationRads;
            return currentRotation;
        }

        void RotateTo(float primaryAxisRotation)
        {
            var drive = articulationBody.xDrive;
            drive.target = primaryAxisRotation;
            articulationBody.xDrive = drive;
        }
    }
}
