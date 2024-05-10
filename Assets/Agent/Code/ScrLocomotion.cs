using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SotomaYorch.Agent.Dijkstra
{
    //this will be the parent class to operate
    //locomotion & routes for the agent
    //the following classes will inherit this functionaluty:
    //     Input -> InputSystem
    //     Dijkstra
    //     A*
    public class ScrLocomotion : MonoBehaviour
    {
        #region KnobsAtTheInspector

        public bool debugOperations;

        #endregion

        #region LocalVariables

        [SerializeField]
        [HideInInspector]
        protected ScrAgent _scrAgent;

        #endregion

        #region InitializationMethods

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                CheckEditorReferences();
            }
        }

        void Start()
        {
            CheckInitializationReferences();
        }

        protected virtual void CheckEditorReferences()
        {
            if (_scrAgent == null)
            {
                _scrAgent = GetComponent<ScrAgent>();
            }
        }

        protected virtual void CheckInitializationReferences()
        {

        }

        #endregion

        #region Getters

        protected string GetDebugHeader
        {
            get
            {
                return gameObject.name + " - " + this.GetType().ToString() + ": ";
            }
        }

        #endregion
    }
}