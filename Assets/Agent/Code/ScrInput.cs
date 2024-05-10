using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

namespace SotomaYorch.Agent.Dijkstra
{
    public class ScrInput : ScrLocomotion
    {
        #region KnobsAtTheInspector

        [SerializeField]
        [HideInInspector]
        protected InputActionAsset _inputActionAsset;

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

        protected override void CheckEditorReferences()
        {
            //Rescue the reference to the agent
            base.CheckEditorReferences();

            //Reference to the input action asset
#if UNITY_EDITOR
            if (_inputActionAsset == null)
            {
                _inputActionAsset = (InputActionAsset)AssetDatabase.LoadAssetAtPath("Assets/Agent/InputActions/AgentInputActions.inputactions", typeof(InputActionAsset));
                if (_inputActionAsset == null) //if the inputactions asset is not found
                {
                    Debug.LogError(GetDebugHeader + " We couldn't find the input actions asset " +
                        "please check that the asset exist at the following route" +
                        "Assets/Agent/Input/AgentInputActions.inputactions");
                }
            } 
#endif
        }

        protected override void CheckInitializationReferences()
        {
            base.CheckInitializationReferences();
            if (_inputActionAsset != null)
            {
                _inputActionAsset.FindActionMap("AgentActionMap").FindAction("MOVE").performed += OnMoveAction;
                _inputActionAsset.FindActionMap("AgentActionMap").FindAction("JUMP").performed += OnJumpAction;
                _inputActionAsset.FindActionMap("AgentActionMap").FindAction("JUMP_RELEASE").performed += OnJumpReleaseAction;
                _inputActionAsset.FindActionMap("AgentActionMap").FindAction("IDLE").performed += OnIdleAction;

                if (debugOperations)
                {
                    Debug.Log(GetDebugHeader + ": " + "Input System fully operational.");
                }
            }
            else
            {
                Debug.Log(GetDebugHeader + ": " + "Missing Input Actions Asset." +
                    " Maybe the asset doesn't exist");
            }
        }

        void OnEnable()
        {
            _inputActionAsset.FindActionMap("AgentActionMap").Enable();
        }
        void OnDisable()
        {
            _inputActionAsset.FindActionMap("AgentActionMap").Disable();
        }

        #endregion

        #region InputActionMethods

        void OnMoveAction(InputAction.CallbackContext context)
        {
            //the interaction was executed
            //it will read changes in the input
            if (context.performed && context.ReadValue<Vector2>().magnitude > 0.0f)
            {
                _scrAgent.MoveActionMechanic(context.ReadValue<Vector2>());

                if (debugOperations)
                {
                    Debug.Log(GetDebugHeader + "MOVE Action Mechanic");
                }
            }
        }

        void OnJumpAction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _scrAgent.JumpActionMechanic();

                if (debugOperations)
                {
                    Debug.Log(GetDebugHeader + "JUMP Action Mechanic");
                }
            }
        }

        void OnJumpReleaseAction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _scrAgent.JumpReleaseActionMechanic();

                if (debugOperations)
                {
                    Debug.Log(GetDebugHeader + "JUMP RELEASE Action Mechanic");
                }
            }
        }

        void OnIdleAction(InputAction.CallbackContext context)
        {
            //the interaction ended
            if (context.performed)
            {
                _scrAgent.IdleActionMechanic();

                if (debugOperations)
                {
                    Debug.Log(GetDebugHeader + "JUMP RELEASE Action Mechanic");
                }
            }
        }

        #endregion
    }
}
