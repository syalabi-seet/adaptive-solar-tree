using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Actuators;

namespace Module
{
    public class ModuleController : MonoBehaviour
    {
        [SerializeField]
        public GameObject[] joints;

        [SerializeField]
        public GameObject[] solarPanels;

        AgentController agentController;

        float upperLimit = 180f;

        void Start()
        {
            agentController = GetComponent<AgentController>();
        }

        public void SetOrientation(ActionBuffers actionBuffers)
        {
            var actions = actionBuffers.ContinuousActions;

            for (int i = 0; i < joints.Length; i++)
            {
                JointController jointController = joints[i].GetComponent<JointController>();
                float actionVal = upperLimit * actions[i];
                jointController.RotateTo(actionVal);
                agentController.AddReward(-0.01f * actionVal);
            }
        }

        public void ResetOrientation()
        {
            for (int i = 0; i < joints.Length; i++)
            {
                JointController jointController = joints[i].GetComponent<JointController>();
                jointController.Reset();
            }
        }
    }
}