using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using Yudiz.XRStarter.Interactions;

namespace Yudiz.XRStarter
{
    public class XRCustomGrabbable : XRGrabInteractable
    {
        [Header("Grab Properties")]
        public Transform leftAnchorTransform;
        public Transform rightAnchorTransform;
        public HandGrabType grabType;
        public bool shouldResetOnRelease;

        private Vector3 grabbedPosition;
        private Quaternion grabbedRotation;

        private SnapZone socketInteractor;
        protected Collider interactableCollider;

        public UnityEvent<XRCustomGrabbable> OnGrabbed;
        public UnityEvent<XRCustomGrabbable> OnReleased;

        #region UNITY_CALLBACKS
        protected override void Awake()
        {
            base.Awake();

            interactableCollider = GetComponent<Collider>();
            if (interactableCollider == null)
            {
                interactableCollider = GetComponentInChildren<Collider>();
            }
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            selectEntered.AddListener(OnItemGrabbed);
            selectExited.AddListener(OnItemReleased);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            selectEntered.AddListener(OnItemGrabbed);
            selectExited.AddListener(OnItemReleased);
        }
        #endregion

        #region METHOD_OVERRIDES
        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            if (leftAnchorTransform == null || rightAnchorTransform == null)
            {
                base.OnSelectEntering(args);
                return;
            }

            if (args.interactorObject.transform.TryGetComponent(out ControllerAvatar controllerAvatar))
            {
                if (controllerAvatar.HandSide == HandSide.Left)
                {
                    if (attachTransform != leftAnchorTransform)
                    {
                        attachTransform = leftAnchorTransform;
                    }
                }
                else
                {
                    if (attachTransform != rightAnchorTransform)
                    {
                        attachTransform = rightAnchorTransform;
                    }
                }
            }

            base.OnSelectEntering(args);
        }
        #endregion

        #region PRIVATE_METHODS
        private void OnItemGrabbed(SelectEnterEventArgs arg0)
        {
            if (shouldResetOnRelease)
            {
                grabbedPosition = transform.position;
                grabbedRotation = transform.rotation;
            }

            OnGrabbed?.Invoke(this);
        }

        private void OnItemReleased(SelectExitEventArgs arg0)
        {
            if (shouldResetOnRelease)
            {
                transform.position = grabbedPosition;
                transform.rotation = grabbedRotation;
            }
            OnReleased?.Invoke(this);
        }
        #endregion

        #region PUBLIC_METHODS
        public void ToggleInteractableCollider(bool enabled)
        {
            if (interactableCollider == null)
                return;
            interactableCollider.enabled = enabled;
        }
        #endregion

#if UNITY_EDITOR
        #region EDITOR_TOOLS_METHODS
        [ContextMenu("AddGrabAdjuster")]
        public void AddGrabAdjuster()
        {
            gameObject.AddComponent<GrabAnchorAdjuster>();
        }

        [ContextMenu("RemoveGrabAdjuster")]
        public void RemoveGrabAdjuster()
        {
            DestroyImmediate(gameObject.GetComponent<GrabAnchorAdjuster>());
        }

        #endregion
#endif

        #region SNAPPING
        public void OnSocketAttached(SelectEnterEventArgs args)
        {
            socketInteractor = args.interactorObject as SnapZone;
        }
        public void OnSocketDetached(SelectExitEventArgs args)
        {
            socketInteractor = null;
        }
        #endregion
    }
}