using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SotomaYorch.Game
{
    public class ScrVictoryGoalMechanic : MonoBehaviour
    {
        #region KnobsAtTheInspector



        #endregion

        #region LocalVariables

        [SerializeField]
        [HideInInspector]
        protected ScrGameReferee _scrGameReferee;

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
            CheckEditorReferences();
        }

        //Editor
        protected void CheckEditorReferences()
        {
            if (_scrGameReferee == null)
            {
                _scrGameReferee = GameObject.Find("GameReferee").GetComponent<ScrGameReferee>();
            }
        }

        //Runtime
        protected void CheckInitializationReferences()
        {

        }

        #endregion

        #region RuntimeEvents

        private void OnTriggerEnter(Collider other)
        {
            //Check if we touched the agent
            if (other.gameObject.name.Contains("Zombie"))
            {
                _scrGameReferee.VictoryGoalConditionMet();
            }
        }

        #endregion
    }
}