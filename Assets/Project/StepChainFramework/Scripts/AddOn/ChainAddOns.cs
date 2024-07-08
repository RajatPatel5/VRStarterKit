using System;
using Yudiz.XRStarter.Interactions;
using Yudiz.XRStarter;
using UnityEngine;

namespace ChainFramework
{
    public class SnappedConditionalStep : IChainStep
    {
        #region Fields


        private Action _completedAction;

        private SnapZone _snapZone;
        private GameObject _snapItem;
        #endregion

        #region Properties
        public bool IsInitiated { get; set; } = false;
        public bool IsCompleted { get; set; } = false;

        #endregion

        #region Constructor

        public SnappedConditionalStep(SnapZone snapZone, GameObject snapItem, Action completedAction = null)
        {
            _snapZone = snapZone;
            _snapItem = snapItem;
            _completedAction = completedAction;
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
                _snapZone.OnDropped.AddListener(OnSnapComplete);
                IsInitiated = true;
            }
        }

        public void AutoComplete()
        {
            IsCompleted = true;
        }
        public void OnSnapComplete(SnapZone snapZone, GameObject snapItem)
        {
            if (_snapItem == snapItem)
            {
                snapZone.OnDropped.RemoveListener(OnSnapComplete);
                _completedAction?.Invoke();
                IsCompleted = true;
            }
        }
        public void Kill()
        {
            _snapZone.OnDropped.RemoveListener(OnSnapComplete);
        }
        #endregion
    }

    public class GrabbedConditionalStep : IChainStep
    {
        #region Fields


        private Action _completedAction;

        private XRCustomGrabbable _grabbable;
        #endregion

        #region Properties
        public bool IsInitiated { get; set; } = false;
        public bool IsCompleted { get; set; } = false;

        #endregion

        #region Constructor
        public GrabbedConditionalStep(XRCustomGrabbable grabbable, Action completedAction = null)
        {
            _grabbable = grabbable;
            _completedAction = completedAction;
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
                _grabbable.OnGrabbed.AddListener(OnGrabbed);
                IsInitiated = true;
            }
        }

        public void AutoComplete()
        {
            IsCompleted = true;
        }
        public void OnGrabbed(XRCustomGrabbable grabbable)
        {
            _grabbable.OnGrabbed.RemoveListener(OnGrabbed);
            _completedAction?.Invoke();
            IsCompleted = true;
        }
        public void Kill()
        {
            _grabbable.OnGrabbed.RemoveListener(OnGrabbed);
        }
        #endregion
    }

    public class UnGrabbedConditionalStep : IChainStep
    {
        #region Fields


        private Action _completedAction;

        private XRCustomGrabbable _grabbable;
        #endregion

        #region Properties
        public bool IsInitiated { get; set; } = false;
        public bool IsCompleted { get; set; } = false;

        #endregion

        #region Constructor
        public UnGrabbedConditionalStep(XRCustomGrabbable grabbable, Action completedAction = null)
        {
            _grabbable = grabbable;
            _completedAction = completedAction;
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
                _grabbable.OnReleased.AddListener(OnUnGrabbed);
                IsInitiated = true;
            }
        }

        public void AutoComplete()
        {
            IsCompleted = true;
        }
        public void OnUnGrabbed(XRCustomGrabbable grabbable)
        {
            _grabbable.OnReleased.RemoveListener(OnUnGrabbed);
            _completedAction?.Invoke();
            IsCompleted = true;
        }
        public void Kill()
        {
            _grabbable.OnReleased.RemoveListener(OnUnGrabbed);
        }
        #endregion
    }

    public class SnapAttachedConditionalStep : IChainStep
    {
        #region Fields
        private Action _completedAction;
        private SnapAttach _snapAttach;
        #endregion

        #region Properties
        public bool IsInitiated { get; set; } = false;
        public bool IsCompleted { get; set; } = false;

        #endregion

        #region Constructor
        public SnapAttachedConditionalStep(SnapAttach snapAttach, Action completedAction = null)
        {
            _snapAttach = snapAttach;
            _completedAction = completedAction;
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
                _snapAttach.OnSnapAttached.AddListener(OnSnapAttached);
                IsInitiated = true;
            }
        }

        public void AutoComplete()
        {
            IsCompleted = true;
        }
        public void OnSnapAttached(SnapAttach snapAttach, Transform snappedItem)
        {
            _snapAttach.OnSnapAttached.RemoveListener(OnSnapAttached);
            _completedAction?.Invoke();
            IsCompleted = true;
        }
        public void Kill()
        {
            _snapAttach.OnSnapAttached.RemoveListener(OnSnapAttached);
        }
        #endregion
    }

    public class SnapDetachedConditionalStep : IChainStep
    {
        #region Fields
        private Action _completedAction;
        private SnapAttach _snapAttach;
        #endregion

        #region Properties
        public bool IsInitiated { get; set; } = false;
        public bool IsCompleted { get; set; } = false;

        #endregion

        #region Constructor
        public SnapDetachedConditionalStep(SnapAttach snapAttach, Action completedAction = null)
        {
            _snapAttach = snapAttach;
            _completedAction = completedAction;
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
                _snapAttach.OnSnapDetached.AddListener(OnSnapDetached);
                IsInitiated = true;
            }
        }

        public void AutoComplete()
        {
            IsCompleted = true;
        }
        public void OnSnapDetached(SnapAttach snapAttach, Transform snappedItem)
        {
            _snapAttach.OnSnapDetached.RemoveListener(OnSnapDetached);
            _completedAction?.Invoke();
            IsCompleted = true;
        }
        public void Kill()
        {
            _snapAttach.OnSnapDetached.RemoveListener(OnSnapDetached);
        }
        #endregion
    }

    public class RotateMaxConditionalStep : IChainStep
    {
        #region Fields
        private Action _completedAction;
        private RotateInteractable _rotateInteractable;
        #endregion

        #region Properties
        public bool IsInitiated { get; set; } = false;
        public bool IsCompleted { get; set; } = false;

        #endregion

        #region Constructor
        public RotateMaxConditionalStep(RotateInteractable rotateInteractable, Action completedAction = null)
        {
            _rotateInteractable = rotateInteractable;
            _completedAction = completedAction;
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
                _rotateInteractable.OnObjectRotationMaxReached.AddListener(OnRotated);
                IsInitiated = true;
            }
        }

        public void AutoComplete()
        {
            IsCompleted = true;
        }
        public void OnRotated(RotateInteractable rotator)
        {
            _rotateInteractable.OnObjectRotationMaxReached.RemoveListener(OnRotated);
            _completedAction?.Invoke();
            IsCompleted = true;
        }
        public void Kill()
        {
            _rotateInteractable.OnObjectRotationMaxReached.RemoveListener(OnRotated);
        }
        #endregion
    }

    public class RotateMinConditionalStep : IChainStep
    {
        #region Fields
        private Action _completedAction;
        private RotateInteractable _rotateInteractable;
        #endregion

        #region Properties
        public bool IsInitiated { get; set; } = false;
        public bool IsCompleted { get; set; } = false;

        #endregion

        #region Constructor
        public RotateMinConditionalStep(RotateInteractable rotateInteractable, Action completedAction = null)
        {
            _rotateInteractable = rotateInteractable;
            _completedAction = completedAction;
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
                _rotateInteractable.OnObjectRotationMinReached.AddListener(OnRotated);
                IsInitiated = true;
            }
        }

        public void AutoComplete()
        {
            IsCompleted = true;
        }
        public void OnRotated(RotateInteractable rotator)
        {
            _rotateInteractable.OnObjectRotationMinReached.RemoveListener(OnRotated);
            _completedAction?.Invoke();
            IsCompleted = true;
        }
        public void Kill()
        {
            _rotateInteractable.OnObjectRotationMinReached.RemoveListener(OnRotated);
        }
        #endregion
    }
}