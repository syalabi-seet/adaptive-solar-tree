using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class ModuleController : MonoBehaviour
    {
        [System.Serializable]
        public struct Joint
        {
            public string inputAxis;
            public GameObject robotPart;
        }
        public Joint[] joints;

        ArticulationJointController jointController;
        MeshCollider meshCollider;

        // Start is called before the first frame update
        void Start()
        {
            meshCollider = gameObject.GetComponent<MeshCollider>();            
        }

        void OnTriggerEnter(Collider other)
        {
            print(other);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void ResetOrientation()
        {
            for (int i = 0; i < joints.Length; i++)
            {
                jointController = joints[i].robotPart.GetComponent<ArticulationJointController>();
                jointController.Reset();            
            }
        }
    }
}