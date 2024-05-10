using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SotomaYorch.Agent
{
    #region Enums

    public enum AgentActionMechanic
    {
        IDLE = 0,
        MOVE = 1,
        JUMP = 2,
        STAND = 3,
        FALL = 4
    }

    //States have priorities, so the most important one
    //will prevail
    public enum AgentState
    {
        NONE = -1,
        IDLEING = 0,
        MOVING = 1,
        JUMPING = 2,
        STANDING = 3,
        FALLING = 4
    }

    #endregion

    #region Structs

    public struct StateTransition
    {
        public AgentState fromState;
        public AgentState toState;
        public bool value;

        public void SetStateTransition(AgentState fromValue, AgentState toValue, bool val)
        {
            fromState = fromValue;
            toState = toValue;
            value = val;
        }
    }

    #endregion

    public class ScrAgent : MonoBehaviour
    {
        #region KnobsAtTheInspector

        public float speedOfTheAgent;

        public bool debugOperations;
        

        #endregion

        #region LocalVariables

        //STATES IN THE AGENT
        protected AgentState _enAgentState;
        protected AgentState _toAgentState;
        protected List<StateTransition> _strTransitionMatrix;

        //REFERENCES FOR THE COMPONENTS IN THE AGENT
        [SerializeField]
        [HideInInspector]
        protected Rigidbody _rb;
        [SerializeField]
        [HideInInspector]
        protected PhysicMaterial _pm;
        [SerializeField]
        [HideInInspector]
        protected Animator _animator;
        [SerializeField]
        [HideInInspector]
        protected GameObject _goDebugState;
        [SerializeField]
        [HideInInspector]
        protected Ray _ray;

        //LOCOMOTION LOCAL VARIABLES
        protected Vector3 _v3MoveDirection;
        protected Vector3 _v3DeltaVelocity;
        protected Vector3 _v3Acceleration;
        protected Vector3 _v3rigidbodyVelocity;
        protected RaycastHit raycastHit;

        //JUMPING LOCAL VARIBLES
        protected bool _bolJumpForce;
        protected bool _bolPreviousJumpForce;

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
            InitializeAgent();
        }

        protected virtual void CheckEditorReferences()
        {
            if (!Application.isPlaying)
            {
                if (_rb == null) //Inspector: "None"
                {
                    _rb = GetComponent<Rigidbody>();
                    if (_rb == null) //if the rigidbody is still null
                    {
                        Debug.LogError(GetDebugHeader + " The gameobject has no " +
                            "rigidbody attached to it, please check the gameobject");
                    }
                }

                if (_pm == null)
                {
                    _pm = GetComponent<CapsuleCollider>().material;
                    if (_pm == null)
                    {
                        Debug.LogError(GetDebugHeader + " The rigidbody has no " +
                            "physics material added, please add one so this can " +
                            "operate properly");
                    }
                }

                if (_animator == null)
                {
                    _animator = GetComponent<Animator>();
                    if (_animator == null) //if the animator is still null
                    {
                        Debug.LogError(GetDebugHeader + " The gameobject has no " +
                            "animator attached to it, please check the gameobject");
                    }
                }

                if (_goDebugState == null)
                {
                    _goDebugState = transform.GetChild(0).gameObject;
                }
            }
        }

        protected virtual void CheckInitializationReferences()
        {
            _pm = new PhysicMaterial();
            GetComponent<CapsuleCollider>().material = _pm;
        }

        protected virtual void InitializeAgent()
        {
            _enAgentState = AgentState.IDLEING;
            _toAgentState = AgentState.NONE;

            _ray = new Ray();

            SetTransitionMatrix();
        }

        protected void SetTransitionMatrix()
        {
            _strTransitionMatrix = new List<StateTransition>();

            //Setted up this transtions by hard code
            //since it's the main DNA for a basic agent

            //FROM IDLE TO OTHER STATES
            SetTransitionMatrixValue(AgentState.IDLEING, AgentState.MOVING, true);
            SetTransitionMatrixValue(AgentState.IDLEING, AgentState.JUMPING, true);
            SetTransitionMatrixValue(AgentState.IDLEING, AgentState.FALLING, true);

            //FROM MOVING TO OTHER STATES
            SetTransitionMatrixValue(AgentState.MOVING, AgentState.IDLEING, true);
            SetTransitionMatrixValue(AgentState.MOVING, AgentState.JUMPING, true);
            SetTransitionMatrixValue(AgentState.MOVING, AgentState.FALLING, true);

            //FROM JUMPING TO OTHER STATES
            SetTransitionMatrixValue(AgentState.JUMPING, AgentState.FALLING, true);

            //FROM STANDING TO OTHER STATES
            SetTransitionMatrixValue(AgentState.STANDING, AgentState.IDLEING, true);
            SetTransitionMatrixValue(AgentState.STANDING, AgentState.FALLING, true);
            SetTransitionMatrixValue(AgentState.STANDING, AgentState.MOVING, true);

            //FROM FALLING TO OTHER STATES
            SetTransitionMatrixValue(AgentState.FALLING, AgentState.STANDING, true);
        }

        protected void SetTransitionMatrixValue(AgentState from, AgentState to, bool val)
        {
            StateTransition value = new StateTransition();
            value.SetStateTransition(from, to, val);
            _strTransitionMatrix.Add(value);
        }

        protected bool CanTransitionToState(AgentState from, AgentActionMechanic to)
        {
            foreach (StateTransition stateTransition in _strTransitionMatrix)
            {
                if (stateTransition.fromState == (AgentState)((int)from) &&
                    stateTransition.toState == (AgentState)((int)to))
                {
                    return stateTransition.value;
                }
            }

            if (debugOperations)
            {
                Debug.Log(GetDebugHeader + " CanTransitionToState(): " +
                    "There is no transition from " + from +
                    " to " + to);
            }
            return false;
        }

        #endregion

        #region OperationMethods

        void Update()
        {
            UpdateAgent();
        }

        protected virtual void UpdateAgent()
        {
            if (debugOperations)
            {
                //Debug.Log(GetDebugHeader + "Current State = " + AgentState);
            }

            switch (_enAgentState)
            {
                case AgentState.IDLEING:
                    Idleing();
                    break;
                case AgentState.MOVING:
                    Moving();
                    break;
                case AgentState.JUMPING:
                    Jumping();
                    break;
                case AgentState.STANDING:
                    Standing();
                    break;
                case AgentState.FALLING:
                    Falling();
                    break;
            }
        }

        public void InvokeActionMechanic(AgentActionMechanic value)
        {
            ////we check if the state mechanic is equals to the one we're
            ////currently transitioning
            //if (value == (AgentActionMechanic)((int)_toAgentState))
            //{
            //    if (debugOperations)
            //    {
            //        Debug.Log(GetDebugHeader + "Received the State Mechanic " + value +
            //            " but we're actually transitioning to that state." +
            //            "So this state mechanic invocation will be ignored.");
            //    }
            //    //since it is the same transition
            //    //we're going to, we exit this method
            //    return;
            //}
            ////we check if the state mechanic is actually the same as
            ////the state we're currently on
            //else if (value == (AgentActionMechanic)((int)_enAgentState))
            //{
            //    if (debugOperations)
            //    {
            //        Debug.Log(GetDebugHeader + "Received the State Mechanic " + value +
            //            " but we're actually at this state." +
            //            "So this state mechanic invocation will be ignored.");
            //    }
            //    //since we're already at this state
            //    //we're going to, we exit this method
            //    return;
            //}
            ////we check if the agent is actually not transitioning to any state
            //else if (_toAgentState == AgentState.NONE)
            //{
            //    if (debugOperations)
            //    {
            //        Debug.Log(GetDebugHeader + "Received the State Mechanic " + value +
            //            " but we're actually at this state." +
            //            "So this state mechanic invocation will be ignored.");
            //    }
            //    //since there is no state to transition to
            //    //the transition will be "automatic"
                
            //    //but before we finish, we have to check if the transition
            //    //is valid from the state we're actually on,
            //    //from the transition matrix
            //    if (CanTransitionToState(_enAgentState, value))
            //    {
            //        //the transition is valid, so we carry on
            //        TransitionToState(value);
            //    }
            //}
            ////so if anything of the previous cases occur
            ////this means that there's actually a transition to another state
            //else
            //{
            //    //check if the state mechanic is more important
            //    //than the state we're transitioning
            //    if ((int)value > (int)_toAgentState)
            //    {
            //        //This new state mechanic has more priority
            //        //than the previous one

            //        //now let's check if the transition is possible within
            //        //the transition matrix
            //        if (CanTransitionToState(_enAgentState, value))
            //        {
            //            //the transition is valid, so we carry on
            //            TransitionToState(value);
            //        }
            //    }
            //}
        }

        public void EndingState(AgentState actualState, AgentState toState)
        {
            switch (actualState)
            {
                case AgentState.IDLEING:
                    //Not gonna happen, it is a looping state
                    break;
                case AgentState.MOVING:
                    //Not gonna happen, it is a looping state
                    break;
                case AgentState.JUMPING:
                    InvokeActionMechanic((AgentActionMechanic)((int)toState));
                    break;
                case AgentState.STANDING:
                    if (_v3MoveDirection.magnitude == 0.0f)
                    {
                        InvokeActionMechanic((AgentActionMechanic)((int)toState));
                    }
                    else
                    {
                        InvokeActionMechanic(AgentActionMechanic.MOVE);
                    }
                    break;
                case AgentState.FALLING:
                    //Not gonna happen, it is a looping state
                    break;
            }
        }

        protected void TransitionToState(AgentActionMechanic value)
        {
            //very important, we have to make sure if there was a previous state
            //it was transitioning to, so we clean the value
            if (_toAgentState != AgentState.NONE)
            {
                _animator.SetBool(_toAgentState.ToString(), false);
            }
            
            //Getting to the state will rely entirely in the
            //animator, so we send the proper invocation
            _animator.SetBool(value.ToString(), true);

            //now we have the new state we're expecting to transition to
            //it's all up now to the animator
            _toAgentState = (AgentState)((int)value);
        }

        public void ArrivedAtAnimatorState(AgentState value)
        {
            //we clean the possible transition,
            //for further state mechanic invocation
            _toAgentState = AgentState.NONE;

            //now we're on a new state
            //where the Update function will operate
            //properly
            _enAgentState = value;
            _goDebugState.name = "STATE: " + _enAgentState.ToString();

            SetNewStateParameters();
        }

        protected void SetNewStateParameters()
        {
            switch (_enAgentState)
            {
                case AgentState.IDLEING:
                    PrepareForIdleingState();
                    break;
                case AgentState.MOVING:
                    PrepareForMovingState();
                    break;
                case AgentState.JUMPING:
                    PrepareForJumpingState();
                    break;
                case AgentState.STANDING:
                    PrepareForStandingState();
                    break;
                case AgentState.FALLING:
                    PrepareForFallingState();
                    break;
            }
        }

        #endregion

        #region StateMethods

        protected virtual void Idleing()
        {
            _rb.WakeUp();

            if (!isGrounded())
            {
                InvokeActionMechanic(AgentActionMechanic.FALL);
            }
            else
            {
                if (_v3MoveDirection.magnitude != 0)
                {
                    InvokeActionMechanic(AgentActionMechanic.MOVE);
                }
            }
        }

        protected virtual void Moving()
        {
            if (!isGrounded())
            {
                InvokeActionMechanic(AgentActionMechanic.FALL);
            }
            else
            {
                LocomoteAgent();
            }
        }

        protected virtual void Jumping()
        {
            if (_bolJumpForce)
            {
                if (!_bolPreviousJumpForce)
                {
                    _rb.AddForce(Vector3.up * 7.5f, ForceMode.VelocityChange);
                    _bolPreviousJumpForce = true;
                }
                else
                {
                    _rb.AddForce(Vector3.up * 2.75f, ForceMode.Acceleration);
                }
            }

            LocomoteAgent();
        }

        protected virtual void Standing()
        {
            _rb.WakeUp();
        }

        protected virtual void Falling()
        {
            if (isGrounded())
            {
                InvokeActionMechanic(AgentActionMechanic.STAND);
            }
            else
            {
                LocomoteAgent();
            }
        }

        #endregion

        #region StateTransitionMethods

        protected virtual void PrepareForIdleingState()
        {
            //put on the breaks at max
            _pm.dynamicFriction = 1.0f;
            _pm.staticFriction = 1.0f;

            //we brake the agent completely
            //_rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        protected virtual void PrepareForMovingState()
        {
            //release the breaks so the agent can move
            _pm.dynamicFriction = 0.0f;
            _pm.staticFriction = 0.0f;
        }

        protected virtual void PrepareForJumpingState()
        {
            _bolPreviousJumpForce = false;
        }

        protected virtual void PrepareForStandingState()
        {
            //put on the breaks at max
            _pm.dynamicFriction = 1.0f;
            _pm.staticFriction = 1.0f;

            //we brake the agent completely
            //_rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        protected virtual void PrepareForFallingState()
        {

        }

        #endregion

        #region ComplementaryMethods

        protected bool isGrounded()
        {
            _ray.origin = transform.position + Vector3.up * 0.1f;
            _ray.direction = Vector3.down;

            if (debugOperations)
            {
                Debug.DrawRay(_ray.origin, _ray.direction * 0.2f, Color.red, Time.deltaTime);
            }

            if (Physics.Raycast(_ray, out raycastHit, 0.2f))
            {
                return true;
            }

            return false;
        }

        protected void LocomoteAgent()
        {
            //Once we know which direction to move,
            //we check the current force of the rigidbody, so we can
            //do some corrections
            _v3rigidbodyVelocity = _rb.velocity;
            _v3rigidbodyVelocity.y = 0.0f;
            _v3DeltaVelocity = (transform.forward * speedOfTheAgent) - _v3rigidbodyVelocity;
            _v3Acceleration = _v3DeltaVelocity / Time.deltaTime;

            if (_v3Acceleration.sqrMagnitude > speedOfTheAgent * speedOfTheAgent)
            {
                _v3Acceleration = _v3Acceleration.normalized * speedOfTheAgent;
            }

            //we make the proper accceleration force
            _rb.AddForce(_v3Acceleration, ForceMode.Acceleration);

            RotateTowardsDirection();
        }

        protected void RotateTowardsDirection()
        {
            //we start to look at the target direction set by the action mechanic
            transform.forward = Vector3.Lerp(transform.forward, _v3MoveDirection, 0.2f);
        }

        #endregion

        #region ActionMechanicEvents

        public void MoveActionMechanic(Vector2 value)
        {
            //the following values will be read by the following states:
            //MOVING
            //FALLING: since this movement will not change to a MOVING-FALLING state
            _v3MoveDirection.x = value.x;
            _v3MoveDirection.z = value.y;
            _v3MoveDirection.y = 0.0f;

            InvokeActionMechanic(AgentActionMechanic.MOVE);

            if (debugOperations)
            {
                Debug.Log(GetDebugHeader + " The moving input IS HAPPENING");
            }
        }

        public void JumpActionMechanic()
        {
            InvokeActionMechanic(AgentActionMechanic.JUMP);
            _bolJumpForce = true;
        }

        public void JumpReleaseActionMechanic()
        {
            _bolJumpForce = false;
            _bolPreviousJumpForce = false;
        }

        public void IdleActionMechanic()
        {
            _v3MoveDirection = Vector3.zero;

            InvokeActionMechanic(AgentActionMechanic.IDLE);

            if (debugOperations)
            {
                Debug.Log(GetDebugHeader + " The moving input STOPPED");
            }
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

        public AgentState AgentState
        {
            get
            {
                return _enAgentState;
            }
        }

        #endregion
    }
}