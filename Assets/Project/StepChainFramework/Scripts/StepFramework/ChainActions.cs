using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Yudiz.XRStarter;

namespace ChainFramework
{
    public interface IChainStep
    {
        #region VARS
        bool IsCompleted { get; set; }
        bool IsInitiated { get; set; }
        #endregion

        #region METHODS
        void OnChainStart();
        void Execute();
        void AutoComplete();
        void Kill();
        #endregion
    }
    public class DoActionStep : IChainStep
    {
        #region Fields

        private Action _action;

        #endregion

        #region Properties

        public bool IsCompleted { get; set; } = false;
        public bool IsInitiated { get; set; } = false;

        #endregion

        #region Constructor

        public DoActionStep(Action action)
        {
            _action = action;
        }

        #endregion

        #region Methods
        public void OnChainStart()
        {

        }

        public void Execute()
        {
            _action();
            IsInitiated = true;
            IsCompleted = true;
        }

        public void AutoComplete()
        {
            _action();
            IsCompleted = true;
        }
        public void Kill() { }
        #endregion
    }

    public class WaitStep : IChainStep
    {
        #region Fields

        private float _waitTime;
        private float _currentWaitTime;
        #endregion

        #region Properties

        public bool IsCompleted { get; set; } = false;
        public bool IsInitiated { get; set; } = false;

        #endregion

        #region Constructor

        public WaitStep(float waitTimeInSeconds)
        {
            _waitTime = waitTimeInSeconds;

            _currentWaitTime = 0;
        }

        #endregion

        #region Methods
        public void OnChainStart()
        {

        }

        public void Execute()
        {

            IsInitiated = true;

            _currentWaitTime += Time.deltaTime;

            if (_currentWaitTime >= _waitTime)
            {
                IsCompleted = true;
            }
        }

        public void AutoComplete()
        {
            IsCompleted = true;
        }
        public void Kill() { }
        #endregion
    }

    public class PlayAudioStep : IChainStep
    {
        #region Fields
        private AudioClip _audioClip;
        private float _length = 0;
        #endregion

        #region Properties

        public bool IsInitiated { get; set; } = false;
        public bool IsCompleted { get; set; } = false;

        #endregion

        #region Constructor

        public PlayAudioStep(AudioClip clip)
        {
            _audioClip = clip;
            _length = clip.length;
        }

        #endregion

        #region Methods
        public void OnChainStart()
        {

        }

        public void Execute()
        {
            if (!IsInitiated)
            {
                AudioManager.Instance.PlayGlobalAudio(_audioClip);
                IsInitiated = true;
            }
            else
            {
                _length -= Time.deltaTime;
                if (_length <= 0)
                {
                    IsCompleted = true;
                }
            }
        }

        public void AutoComplete()
        {
            IsCompleted = true;
        }
        public void Kill()
        {
            AudioManager.Instance.StopGlobalAudio(_audioClip);
        }
        #endregion
    }

    public class PlayRepeatAudioStep : IChainStep
    {
        #region Fields
        private AudioClip _audioClip;
        private float _length = 0;
        private int _repeatTimer;
        private Action _repeatCallback;

        private int totalWaitTime;
        #endregion

        #region Properties

        public bool IsInitiated { get; set; } = false;
        public bool IsCompleted { get; set; } = false;

        #endregion

        #region Constructor

        public PlayRepeatAudioStep(AudioClip clip, int repeatTimer, Action repeatCallback = null)
        {
            _audioClip = clip;
            _length = clip.length;
            _repeatTimer = repeatTimer;
            _repeatCallback = repeatCallback;

            totalWaitTime = (Mathf.CeilToInt(_length) + repeatTimer) * 1000;
        }

        #endregion

        #region Methods
        public void OnChainStart()
        {
            PlayRepeatingAudio();
            IsInitiated = true;
        }

        public void Execute()
        {
            IsCompleted = true;
            AudioManager.Instance.StopGlobalAudio(_audioClip);
        }

        public void AutoComplete()
        {
            IsCompleted = true;
        }
        public void Kill()
        {
            AudioManager.Instance.StopGlobalAudio(_audioClip);
        }
        public async void PlayRepeatingAudio()
        {
            await Task.Delay(totalWaitTime);

            if (!IsCompleted)
            {
                AudioManager.Instance.PlayGlobalAudio(_audioClip);
                _repeatCallback?.Invoke();
                PlayRepeatingAudio();
            }
        }
        #endregion
    }

    public class ConditionalCompletionStep : IChainStep
    {
        #region Fields

        private Action _trueAction;

        private Action _falseAction;

        private Func<bool> _condition;
        #endregion

        #region Properties

        public bool IsInitiated { get; set; } = false;
        public bool IsCompleted { get; set; } = false;

        #endregion

        #region Constructor

        public ConditionalCompletionStep(Action trueAction, Action falseAction, Func<bool> condition)
        {
            _trueAction = trueAction;
            _falseAction = falseAction;
            _condition = condition;
        }

        #endregion

        #region Methods
        public void OnChainStart()
        {

        }

        public void Execute()
        {
            IsInitiated = true;
            if (_condition.Invoke())
            {
                _trueAction.Invoke();
                IsCompleted = true;
            }
            else
            {
                _falseAction.Invoke();
            }
        }

        public void AutoComplete()
        {
            _trueAction.Invoke();
            IsCompleted = true;
        }
        public void Kill() { }
        #endregion
    }

}