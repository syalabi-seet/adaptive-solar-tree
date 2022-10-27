using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    [AddComponentMenu("ML Agents/Joint Controller")]
    public class JointController : MonoBehaviour
    {
        ArticulationBody articulationBody;
        AgentController agentController;
        ArticulationReducedSpace originalPosition;

        // Start is called before the first frame update
        void Start()
        {
            articulationBody = GetComponent<ArticulationBody>();
            agentController = gameObject.GetComponentInParent<AgentController>();
            originalPosition = articulationBody.jointPosition;
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

        public void ResetJoint()
        {       
            
        }

        void OnCollisionEnter(Collision collision)
        {
            agentController.SetReward(-1f);  
            agentController.EndEpisode();                
            print(collision.collider); 
            print(agentController.StepCount);                 
        }
    }
}