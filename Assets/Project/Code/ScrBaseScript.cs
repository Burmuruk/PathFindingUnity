using UnityEngine;
using SotomaYorch.Agent;
//using UnityEngine.InputSystem;
//using UnityEngine.Editor;
//using UnityEngine.

#region HolaMundo

//Namespace = Librerías => DLL's (Dynamic Link Libraries
//Metáfora => ZIP's o Unitpackeages de código
namespace SotomaYorch.BaseCode.Mechanics
{
    public class ScrBaseScript : MonoBehaviour
    {
        //Parámetros visibles en el Inspector
        //public 
        #region Knobs

        public Transform destinationPosition;

        #endregion

        //Variables protegidas o privadas
        //Variables locales para el funcionamiento
        //interno y exclusivo de esta clase
        #region LocalVariables

        protected int _intCalcluationParameter;

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

        }

        protected virtual void CheckInitializationReferences()
        {

        }

        #endregion

        #region OperationMethods



        void Update()
        {

        }

        #endregion

        //GETTERS
        #region Getters



        #endregion
    }

}

#endregion