using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SotomaYorch.Agent.Dijkstra
{
    public enum FindNodesType
    {
        ALL_IN_THE_SCENE,
        BROTHER_NODE
    }

    public class ScrDijkstra : ScrLocomotion
    {
        #region KnobsAtTheInspector

        public FindNodesType findNodesType;

        #endregion

        #region LocalVariables

        //since this belongs to the agent,
        //we will collect all the nodes here
        [SerializeField]
        //[HideInInspector]
        protected List<ScrNode> _scrNodes;

        protected ScrNode _scrNode;

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
            StartAlgorithm();
        }

        //Editor
        protected override void CheckEditorReferences()
        {
            //Rescue the reference to the agent
            base.CheckEditorReferences();

            //Rescue the nodes of the level
            if (_scrNodes != null)
            {
                _scrNodes = new List<ScrNode>();
            }
            if (_scrNodes.Count <= 0)
            {
                switch (findNodesType)
                {
                    case FindNodesType.ALL_IN_THE_SCENE:
                        //Recollect all of the nodes of the level
                        foreach (ScrNode node in GameObject.FindObjectsOfType<ScrNode>())
                        {
                            _scrNodes.Add(node);
                        }
                        break;
                    case FindNodesType.BROTHER_NODE:
                        
                        //Recollect all of the nodes, being child of our own parent
                        foreach (ScrNode node in transform.parent.GetComponentsInChildren<ScrNode>())
                        {
                            _scrNodes.Add(node);
                        }
                        break;
                }
            }
        }

        //Runtime
        protected override void CheckInitializationReferences()
        {

        }

        protected void StartAlgorithm()
        {
            if (_scrNodes.Count > 0)
            {
                //TODO: setup everything

                //Get the first node we will move at
                //obtain a direction the agent will move towards that target


                //change the state to MOVE,
                //via the action mechanic
                //_scrAgent.MoveActionMechanic(calculatedDirection);
            }
        }

        #endregion

        #region RuntimeMethods

        private void Update()
        {
            //calculate in realtime the direction the agent must be targeting towards
            //_scrAgent.MoveActionMechanic(calculatedDirection);
        }

        private void OnTriggerEnter(Collider other)
        {
            //The agent gets in contact with a "DijkstraNode"
            if (other.gameObject.layer == 31)
            {
                //Check if the node in contact is the one we were headed at
                _scrNode = other.gameObject.GetComponent<ScrNode>();

                //if we reach the last node, the agent will stop
                /*
                if (condition)
                {
                    _scrAgent.IdleActionMechanic();
                }
                */
            }
        }

        #endregion

    }
}