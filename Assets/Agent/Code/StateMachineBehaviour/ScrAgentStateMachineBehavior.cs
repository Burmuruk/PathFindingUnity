using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SotomaYorch.Agent
{
    public class ScrAgentStateMachineBehavior : StateMachineBehaviour
    {
        #region Knobs

        public AgentState agentState;
        public bool isLoopAnimation = true;
        public AgentState toStateWhenFinishedAnimation = AgentState.NONE;
        public bool debugOperations;

        #endregion

        #region LocalVariables

        //general references
        protected GameObject _gameObject;
        protected ScrAgent _scrAgent;
        protected AgentActionMechanic _enAgentActionMechanic;

        //variables for calculating current animation time
        protected AnimatorStateInfo _animationState;
        protected AnimatorClipInfo[] _animatorClip;
        protected float _currentTime;

        #endregion

        #region InitializationMethods

        private void Awake()
        {
            _enAgentActionMechanic = (AgentActionMechanic)((int)agentState);
        }

        #endregion

        #region OnStateMethods

         override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //since we don't know which gameObject is attached for this script,
            //we'll wait for the very first call to get the reference
            //also we get the reference to the agent's script
            if (_gameObject == null)
            {
                _gameObject = animator.gameObject;
                _scrAgent = _gameObject.GetComponent<ScrAgent>();
            }

            //now that we entered this state we have to notify the agent that now this is the new state
            //NOTE: This is the only outer code which can set the value for a state
            _scrAgent.ArrivedAtAnimatorState(agentState);
            //we have to clean the animator parameter, so it can receive a new event
            animator.SetBool(_enAgentActionMechanic.ToString(), false);

            if (debugOperations)
            {
                Debug.Log(GetDebugHeader + "OnStateEnter(): " + "We're now ENTERING the state " + agentState.ToString() + " in the animator");
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //if (debugOperations)
            //{
            //    Debug.Log(GetDebugHeader + "OnStateEnter(): " + "The state " + agentState.ToString() + " is UPDATING in the animator");
            //}

            //if (!isLoopAnimation)
            //{
            //    //if the current animation is farther than the 90% of its completion
            //    if (GetCurrentAnimatorTime(animator) > 0.9f)
            //    {
            //        if (_scrAgent.AgentState == agentState)
            //        {
            //            _scrAgent.EndingState(agentState, toStateWhenFinishedAnimation);
            //        }
            //    }
            //}
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //we don't have to notify the Agent code, since the new state will notify
            //about the change in the state
            if (debugOperations)
            {
                Debug.Log(GetDebugHeader + "OnStateExit(): " + "We're LEAVING the state " + agentState.ToString() + " in the animator");
            }
        }

        #endregion

        #region LocalMethods

        protected float GetCurrentAnimatorTime(Animator value, int layer = 0)
        {
            _animationState = value.GetCurrentAnimatorStateInfo(layer);
            _animatorClip = value.GetCurrentAnimatorClipInfo(0);
            _currentTime = _animatorClip[0].clip.length * _animatorClip[0].clip.length;
            return _currentTime;
        }

        #endregion

        #region

        protected string GetDebugHeader
        {
            get
            {
                return (_gameObject != null ? _gameObject.name : "Game Object not known yet") + " - " + this.GetType().ToString() + ": ";
            }
        }

        #endregion
    }
}