using ChainFramework;
using Palmmedia.ReportGenerator.Core.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StepFramework
{
    public class InternalStepsHandler
    {
        #region VARS
        public bool IsValid { get => _enumValues != null && _enumValues.Length > 0; }
        public bool IsCompleted { get => isCompleted; }
        public Chain internalStepChain;

        private Array _enumValues;
        private int _currentStep = 0;
        private int _previousStep = 0;
        private bool isCompleted;
        #endregion

        #region EVENT_DELEGATES
        // Complete Step
        public delegate void CompleteStepEventDelegate(InternalStepsHandler sender, int completedStepIndex);

        public event CompleteStepEventDelegate OnCompleteStepEvent;

        private void OnCompleteStep(int completedStepIndex)
        {
            OnCompleteStepEvent?.Invoke(this, completedStepIndex);
        }

        //Entered New Step
        public delegate void EnterStepEventDelegate(InternalStepsHandler sender, int previousStepIndex, int enteredStepIndex);

        public event EnterStepEventDelegate OnEnterStepEvent;

        private void OnEnterStep(int previousStepIndex, int enteredStepIndex)
        {
            OnEnterStepEvent?.Invoke(this, previousStepIndex, enteredStepIndex);
        }

        //All Internal Steps Completed
        public delegate void AllInternalStepCompleteEventDelegate();

        public event AllInternalStepCompleteEventDelegate OnAllStepsCompletedEvent;

        private void OnAllInternalStepsComplete()
        {
            OnAllStepsCompletedEvent?.Invoke();
        }
        #endregion

        #region METHODS
        public void Initialize(Array enumValues)
        {
            _enumValues = enumValues;
            if (_enumValues.Length == 1 && (int)_enumValues.GetValue(0) == -1)
            {
                Debug.LogError("Trying to Initialize internal step handler with no steps except None = -1, IGNORED!");
                return;
            }
            if (_enumValues.Length > 0)
            {
                isCompleted = false;
                StartStepping();
            }
            else
            {
                Debug.LogError("Trying to Initialize internal step handler with no steps, IGNORED!");
                return;
            }
        }

        public void StartStepping()
        {
            _currentStep = -1;
            _previousStep = -1;

            StartNextStep();
        }
        public void StartNextStep()
        {
            _previousStep = _currentStep;
            _currentStep++;

            if (_currentStep >= _enumValues.GetUpperBound(0) || (int)_enumValues.GetValue(Math.Min(_enumValues.GetUpperBound(0), _currentStep)) == -1)
            {
                //Completed 
                isCompleted = true;
                OnAllInternalStepsComplete();
            }
            else
            {
                internalStepChain?.Kill();
                internalStepChain = ChainManager.Get();

                
                OnEnterStep(_previousStep, _currentStep);

            }
        }

        public void CompleteInternalStep()
        {
            OnCompleteStep(_currentStep);
            StartNextStep();
        }
        #endregion
    }
}