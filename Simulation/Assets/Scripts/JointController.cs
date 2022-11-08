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
        ArticulationReducedSpace originalAcceleration;
        ArticulationReducedSpace originalForce;
        ArticulationReducedSpace originalVelocity;

        public float lowerLimit = -180f;
        public float upperLimit = 180f;
        public float jointSpeed = 300f;

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
            drive.target = Random.Range(lowerLimit, upperLimit);
            // drive.target = 0f;
            articulationBody.xDrive = drive;

            articulationBody.jointPosition = new ArticulationReducedSpace(0f, 0f, 0f);
            articulationBody.jointAcceleration = new ArticulationReducedSpace(0f, 0f, 0f);
            articulationBody.jointForce = new ArticulationReducedSpace(0f, 0f, 0f);
            articulationBody.jointVelocity = new ArticulationReducedSpace(0f, 0f, 0f);           

            articulationBody.velocity = Vector3.zero;
            articulationBody.angularVelocity = Vector3.zero;

            articulationBody.ResetCenterOfMass();
            articulationBody.ResetInertiaTensor();
        }

        void OnCollisionEnter(Collision collision)
        {
            agentController.SetReward(-1f);
            agentController.EndEpisode();
        }
    }
}