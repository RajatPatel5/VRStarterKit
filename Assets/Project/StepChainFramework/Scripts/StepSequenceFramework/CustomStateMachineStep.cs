using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChainFramework;

namespace StepFramework
{
    public class CustomStateMachineStep : ScriptableObject
    {
        #region VARS
        protected Chain _mainStepChain;
        private StepStateMachine _stateMachine;
        protected InternalStepsHandler _internalStepHandler;
        #endregion

        #region METHODS
        public virtual void Init(StepStateMachine stateMachine, CustomSimulation customSim)
        {
            _stateMachine = stateMachine;
        }
        public virtual void OnStepEnter()
        {
            _mainStepChain = ChainManager.Get();

            _mainStepChain.chainCompletedEvent += HandleStepComplete;


            _internalStepHandler = new InternalStepsHandler();
            _internalStepHandler.OnEnterStepEvent += OnInternalStepEnter;
            _internalStepHandler.OnCompleteStepEvent += OnInternalStepComplete;
            _internalStepHandler.OnAllStepsCompletedEvent += HandleStepComplete;
        }

        public virtual void OnStepExit()
        {
            _mainStepChain.chainCompletedEvent -= HandleStepComplete;
            _internalStepHandler.OnEnterStepEvent -= OnInternalStepEnter;
            _internalStepHandler.OnCompleteStepEvent -= OnInternalStepComplete;
            _internalStepHandler.OnAllStepsCompletedEvent -= HandleStepComplete;
        }
        public virtual void OnInternalStepEnter(InternalStepsHandler stepHandler, int previousStepIndex, int enteredStepIndex)
        {

        }
        public virtual void OnInternalStepComplete(InternalStepsHandler stepHandler, int completedStepIndex)
        {

        }

        private void HandleStepComplete()
        {

            if (_internalStepHandler.IsValid)
            {
                if (_internalStepHandler.IsCompleted)
                {
                    _stateMachine.StartNextStep();
                }
            }
            else if (_mainStepChain.HasStarted && _mainStepChain.IsFinished)
            {
                _stateMachine.StartNextStep();
            }
        }
        #endregion
    }
}