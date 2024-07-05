using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainFramework
{
    public class Chain
    {
        #region PUBLIC_VARS

        public bool IsFinished { get => _actions.Count <= 0 && _currentAction == null; }
        public bool HasStarted { get => _hasStarted; }
        #endregion

        #region PRIVATE_VARS

        private Queue<IChainStep> _actions;

        private IChainStep _currentAction;

        private Action _startAction;

        private Action _completeAction;
        private bool _hasStarted;

        #endregion

        #region EVENT_DELEGATES
        public delegate void ChainCompletedEvent();

        public event ChainCompletedEvent chainCompletedEvent;

        public void OnChainCompleted()
        {
            chainCompletedEvent?.Invoke();
        }
        #endregion

        #region CONSTRUCTORS
        public Chain()
        {
            _actions = new Queue<IChainStep>();
        }
        #endregion

        #region METHODS

        public void CompleteCurrentAction()
        {
            _currentAction?.Kill();
            _currentAction = null;
        }

        public void SetStartAction(Action action)
        {
            _startAction = action;
        }

        public void AddAction(IChainStep action)
        {
            if (action != null)
            {
                _actions.Enqueue(action);
            }
        }
        public void SetCompleteAction(Action action)
        {
            _completeAction = action;
        }

        public void Run()
        {
            if (_hasStarted == false)
            {
                OnStart();
                _hasStarted = true;
            }

            if (_currentAction == null && _actions.Count > 0)
            {
                _currentAction = _actions.Dequeue();

                try
                {
                    _currentAction.Execute();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }


            }
            else if (_currentAction != null)
            {
                try
                {
                    _currentAction.Execute();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            if (_currentAction != null)
            {
                if (_currentAction.IsCompleted)
                {
                    CompleteCurrentAction();
                }
            }

            if (IsFinished)
            {
                OnComplete();
            }
        }
        public void Kill()
        {
            if (_currentAction != null)
            {
                _currentAction.Kill();
                _currentAction = null;
            }

            ChainManager.Kill(this);
        }
        private void OnStart()
        {
            if (_startAction != null)
            {
                _startAction();
            }

            foreach (IChainStep step in _actions)
            {
                step.OnChainStart();
            }
        }
        private void OnComplete()
        {
            if (_completeAction != null)
            {
                _completeAction();
            }
            OnChainCompleted();
        }
        public void AutoCompleteChain()
        {
            if (_actions == null || _actions.Count == 0) return;

            IChainStep action = _actions.Dequeue();
            while (action != null)
            {
                action.AutoComplete();
                action.Kill();
                if (_actions.Count > 0)
                    action = _actions.Dequeue();
                else
                    action = null;
            }

            _actions.Clear();
        }
        #endregion
    }
}