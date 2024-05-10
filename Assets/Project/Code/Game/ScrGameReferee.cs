using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;

namespace SotomaYorch.Game
{
    #region Enums

    public enum GameState
    {
        ONGOING,
        VICTORY,
        LOSE,
        PLAYER1_VICTORY,
        PLAYER2_VICTORY,
        PLAYER3_VICTORY,
        PLAYER4_VICTORY,
        DRAW,
        PAUSE
    }

    #endregion

    public class ScrGameReferee : MonoBehaviour
    {
        #region KnobsAtTheInspector

        public float gameTime = 30.0f;
        public bool debugOperations;

        #endregion

        #region LocalVariables

        [SerializeField]
        [HideInInspector]
        protected InputActionAsset _inputActionAsset;
        [SerializeField]
        [HideInInspector]
        protected GameObject _goPivotCamera;
        [SerializeField]
        [HideInInspector]
        protected GameObject _goPauseScreen;
        [SerializeField]
        [HideInInspector]
        protected GameObject _goLoseScreen;
        [SerializeField]
        [HideInInspector]
        protected GameObject _goVictoryScreen;
        [SerializeField]
        [HideInInspector]
        protected TextMeshPro _txtScore;
        [SerializeField]
        [HideInInspector]
        protected TextMeshPro _txtTimeRemaining;

        protected GameState _enGameState;
        protected int _intScore;
        protected float _fltCronometer;

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
            StartGame();
            _enGameState = GameState.ONGOING;
        }

        void StartGame()
        {
            if (_inputActionAsset != null)
            {
                _inputActionAsset.FindActionMap("UIActionMap").FindAction("START").performed += OnStartAction;
                _inputActionAsset.FindActionMap("UIActionMap").FindAction("SELECT").performed += OnSelectAction;
            }

            _fltCronometer = gameTime;
        }

        //Editor
        protected void CheckEditorReferences()
        {
            if (_goPivotCamera == null)
            {
                _goPivotCamera = GameObject.Find("PivotCamera");
            }

            if (_goPauseScreen == null)
            {
                _goPauseScreen = _goPivotCamera.transform.GetChild(0).GetChild(1).gameObject;
            }

            if (_goLoseScreen == null)
            {
                _goLoseScreen = _goPivotCamera.transform.GetChild(0).GetChild(3).gameObject;
            }

            if (_goVictoryScreen == null)
            {
                _goVictoryScreen = _goPivotCamera.transform.GetChild(0).GetChild(4).gameObject;
            }

            if (_txtScore == null)
            {
                _txtScore = GameObject.Find("TxtScore").GetComponent<TextMeshPro>();
            }

            if (_txtTimeRemaining == null)
            {
                _txtTimeRemaining = GameObject.Find("TxtTimeRemaining").GetComponent<TextMeshPro>();
            }

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

        //Runtime
        protected void CheckInitializationReferences()
        {

        }

        void OnEnable()
        {
            CheckEditorReferences();
            _inputActionAsset.FindActionMap("UIActionMap").Enable();
        }

        void OnDisable()
        {
            _inputActionAsset.FindActionMap("UIActionMap").Disable();
        }

        #endregion

        #region RuntimeEvents

        private void Update()
        {
            if (_enGameState == GameState.ONGOING)
            {
                _fltCronometer -= Time.deltaTime;
                _txtTimeRemaining.text = _fltCronometer.ToString("00");

                if (_fltCronometer <= 0.0f)
                {
                    LoseGoalMechanicConditionMet();
                }
            }
        }

        #endregion

        #region MechanicEvents

        public void AddScore(int score)
        {
            if (_enGameState == GameState.ONGOING)
            {
                _intScore += score;
                int score2 = int.Parse(_txtScore.text) + score;
                _txtScore.text = _intScore.ToString("00");

                if (debugOperations)
                {
                    Debug.Log(GetDebugHeader + "Score was added by " +
                        score + " points");
                }
            }
        }

        public void LoseGoalMechanicConditionMet()
        {
            _enGameState = GameState.LOSE;
            _goLoseScreen.SetActive(true);

            if (debugOperations)
            {
                Debug.Log(GetDebugHeader + "The LOSE Goal Mechanic condition was met");
            }
        }

        public void VictoryGoalConditionMet()
        {
            _enGameState = GameState.VICTORY;
            _goVictoryScreen.SetActive(true);

            Invoke("Change_Scene", 1);

            if (debugOperations)
            {
                Debug.Log(GetDebugHeader + "The VICTORY Goal Mechanic condition was met");
            }
        }

        private void Change_Scene()
        {
            try
            {
                print("scene" + SceneManager.GetActiveScene().buildIndex + 1);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            catch (System.Exception)
            {
                print("No more levels");
            }
        }

        #endregion

        #region InputActionEvents

        public void OnStartAction(InputAction.CallbackContext context)
        {
            if (debugOperations)
            {
                Debug.Log(GetDebugHeader + "START Action Mechanic was triggered");
            }

            switch (_enGameState)
            {
                case GameState.LOSE:
                    RestartLevel();
                    break;
                case GameState.VICTORY:
                    NextLevel();
                    break;
                case GameState.PAUSE:
                    ResumeGame();
                    break;
                case GameState.ONGOING:
                    PauseGame();
                    break;
            }
        }

        public void OnSelectAction(InputAction.CallbackContext context)
        {
            RestartLevel();
        }

        #endregion

        #region MechanicEvents

        public void VictoryGoalMechanic()
        {
            NextLevel();
        }

        public void LoseGoalMechanic()
        {
            RestartLevel();
        }

        #endregion

        #region OtherMethods

        protected void ResumeGame()
        {
            Time.timeScale = 1.0f;
            _goPauseScreen.SetActive(false);
            _enGameState = GameState.ONGOING;

            if (debugOperations)
            {
                Debug.Log(GetDebugHeader + "The Game will be PAUSED");
            }
        }

        protected void PauseGame()
        {
            Time.timeScale = 0.0f;
            _goPauseScreen.SetActive(true);
            _enGameState = GameState.PAUSE;

            if (debugOperations)
            {
                Debug.Log(GetDebugHeader + "The Game will be RESUMED");
            }
        }

        protected void NextLevel()
        {
            if (debugOperations)
            {
                Debug.Log(GetDebugHeader + "The NEXT LEVEL will be loaded");
            }

            if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                try
                {
                    SceneManager.LoadScene(0);
                }
                catch
                {
                    Debug.LogError("There are no scenes added at the Build Settings."
                        + " Please add one");
                }
            }
        }

        protected void RestartLevel()
        {
            if (debugOperations)
            {
                Debug.Log(GetDebugHeader + "The LEVEL will be reloaded");
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        #endregion

        #region Getters

        public GameState GetGameState
        {
            get
            {
                return _enGameState;
            }
        }

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