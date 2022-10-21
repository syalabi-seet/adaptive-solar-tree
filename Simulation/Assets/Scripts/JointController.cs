using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class JointController : MonoBehaviour
    {
        ArticulationBody articulationBody;
        AgentController agentController;

        // Start is called before the first frame update
        void Start()
        {
            articulationBody = GetComponent<ArticulationBody>();
            agentController = gameObject.GetComponentInParent<AgentController>();
        }
        
        float CurrentRotation()
        {
            float currentRotationRads = articulationBody.jointPosition[0];
            float currentRotation = Mathf.Rad2Deg * currentRotationRads;
            return currentRotation;
        }

        public void RotateTo(float targetRotation)
        {
            var drive = articulationBody.xDrive;
            drive.target = CurrentRotation() + (targetRotation * Time.fixedDeltaTime);
            articulationBody.xDrive = drive;
        }

        public void Reset()
        {
            var drive = articulationBody.xDrive;
            drive.target = 0f;
            articulationBody.xDrive = drive;
        }

        void OnCollisionEnter(Collision collision)
        {
            agentController.EndEpisode();
            agentController.AddReward(-5f);
        }
    }
}