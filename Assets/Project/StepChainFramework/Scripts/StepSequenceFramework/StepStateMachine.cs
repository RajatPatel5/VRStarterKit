using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StepFramework
{
    public class StepStateMachine : MonoBehaviour
    {
        #region VARS
        public CustomSimulation customSimulation;
        public List<CustomStateMachineStep> steps;

        private int _currentStepIndex;

        private bool isStarted;
        private bool isFinished;
        #endregion

        #region EVENT_DELEGATES
        public delegate void StepsStateMachineCompleteEvent();

        public event StepsStateMachineCompleteEvent stepsStateMachineCompleteEvent;

        public void OnStateMachineCompleted()
        {
            stepsStateMachineCompleteEvent?.Invoke();
        }
        #endregion

        #region METHODS
        private void Awake()
        {
            Init();
        }
        private void Start()
        {
            StartStateMachine();
        }
        public void Init()
        {
            foreach (var step in steps)
            {
                step.Init(this, customSimulation);
            }
        }
        public void SetInitialStep(int stepIndex)
        {
            if (isStarted) { return; }
            _currentStepIndex = stepIndex;
        }
        public void StartStateMachine()
        {
            if (isStarted) { return; }

            SetInitialStep(customSimulation.startingStepIndex);
            isStarted = true;
            isFinished = false;

            steps[_currentStepIndex].OnStepEnter();
        }
        public void StartNextStep()
        {
            steps[_currentStepIndex].OnStepExit();

            _currentStepIndex++;

            if (_currentStepIndex >= steps.Count)
            {
                //Steps Completed
                CompleteStateMachine();
            }
            else
            {
                steps[_currentStepIndex].OnStepEnter();
            }
        }
        public void CompleteStateMachine()
        {
            isStarted = false;
            isFinished = true;
            OnStateMachineCompleted();
        }
        #endregion
    }
}