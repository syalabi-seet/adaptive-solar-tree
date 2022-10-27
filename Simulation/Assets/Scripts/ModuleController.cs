using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Actuators;

namespace Module
{
    [AddComponentMenu("ML Agents/Module Controller")]
    public class ModuleController : MonoBehaviour
    {
        [System.Serializable]
        public struct Joint
        {
            public string inputAxis;
            public GameObject robotPart;
            public float jointSpeed;
        }
        public Joint[] joints;

        [SerializeField]
        public GameObject[] solarPanels;

        AgentController agentController;

        void Start()
        {
            agentController = GetComponent<AgentController>();
        }

        public void SetJoints(ActionBuffers actionBuffers)
        {
            var actions = actionBuffers.ContinuousActions;

            for (int i = 0; i < joints.Length; i++)
            {
                JointController jointController = joints[i].robotPart.GetComponent<JointController>();
                float actionVal = actions[i] * joints[i].jointSpeed;
                jointController.RotateTo(actionVal);
            }
        }

        public void ResetJoints()
        {
            for (int i = 0; i < joints.Length; i++)
            {
                JointController jointController = joints[i].robotPart.GetComponent<JointController>();
                jointController.ResetJoint();
            }
        }
    }
}