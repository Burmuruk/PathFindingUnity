using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

public class ScrPivotCamera : MonoBehaviour
{
    #region LocalVariables

    [SerializeField]
    [HideInInspector]
    protected InputActionAsset _inputActionAsset;

    #endregion

    #region IntializationMethods

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            CheckEditorReferences();
        }
    }

    protected void CheckEditorReferences()
    {
        //Reference to the input action asset
#if UNITY_EDITOR
        if (_inputActionAsset == null)
        {
            _inputActionAsset = (InputActionAsset)AssetDatabase.LoadAssetAtPath("Assets/Project/InputActions/UIInputActions.inputactions", typeof(InputActionAsset));
            if (_inputActionAsset == null) //if the inputactions asset is not found
            {
                Debug.LogError(GetDebugHeader + " We couldn't find the input actions asset " +
                    "please check that the asset exist at the following route" +
                    "Assets/Agent/Input/AgentInputActions.inputactions");
            }
        } 
#endif
    }

    void Start()
    {
        if (_inputActionAsset != null)
        {
            _inputActionAsset.FindActionMap("CameraActionMap").FindAction("MOVE").performed += OnMoveAction;
        }
    }

    void OnEnable()
    {
        if (_inputActionAsset == null)
        {
            CheckEditorReferences();
        }
        _inputActionAsset.FindActionMap("CameraActionMap").Enable();
    }

    void OnDisable()
    {
        if (_inputActionAsset == null)
        {
            CheckEditorReferences();
        }
        _inputActionAsset.FindActionMap("CameraActionMap").Disable();
    }

    #endregion

    #region InputActionMethods

    public void OnMoveAction(InputAction.CallbackContext context)
    {
        float rotation = context.ReadValue<Vector2>().x;

        transform.Rotate(rotation * -10.0f * Vector3.up);
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
