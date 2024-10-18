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
        public Transform secondaryLeftAnchorTransform;
        public Transform secondaryRightAnchorTransform;
        public HandGrabType grabType;
        public bool shouldResetOnRelease;

        [Header("Snap Properties")]
        public Transform snappableTransform;

        [Header("Events")]
        public UnityEvent<XRCustomGrabbable> OnGrabbed;
        public UnityEvent<XRCustomGrabbable> OnReleased;
        public UnityEvent<XRCustomGrabbable> OnGrabbedSecondary;
        public UnityEvent<XRCustomGrabbable> OnReleasedSecondary;


        private Vector3 grabbedPosition;
        private Quaternion grabbedRotation;

        private SnapZone socketInteractor;
        protected Collider interactableCollider;

        private Transform primaryInteractor;
        private Transform secondaryInteractor;

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
        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            if (leftAnchorTransform == null || rightAnchorTransform == null)
            {
                base.OnHoverEntered(args);
                return;
            }
            if (interactorsSelecting.Count == 0)
            {
                base.OnHoverEntered(args);
                return;
            }
            Debug.Log("OnHoverEntered " + interactorsSelecting.Count + " " + interactorsHovering.Count);

            if (args.interactorObject.transform.TryGetComponent(out ControllerAvatar controllerAvatar))
            {
                if (controllerAvatar.HandSide == HandSide.Left)
                {
                    secondaryAttachTransform = secondaryLeftAnchorTransform;
                }
                else
                {
                    secondaryAttachTransform = secondaryRightAnchorTransform;
                }
                if(secondaryInteractor == null)
                {
                    secondaryInteractor = args.interactorObject.transform;
                }
            }
            base.OnHoverEntered(args);
        }
        // protected override void OnHoverExited(HoverExitEventArgs args)
        // {
        //     if (leftAnchorTransform == null || rightAnchorTransform == null)
        //     {
        //         base.OnHoverExited(args);
        //         return;
        //     }

        //     if (secondaryInteractor != null && args.interactorObject.transform == secondaryInteractor)
        //     {
        //         secondaryAttachTransform = null;
        //         secondaryInteractor = null;
        //         Debug.Log("Secondary Interactor Reset");
        //     }
        //     else if (primaryInteractor != null && args.interactorObject.transform == primaryInteractor)
        //     {
        //         if (secondaryInteractor != null)
        //         {
        //             attachTransform = secondaryAttachTransform;
        //             secondaryAttachTransform = null;
        //             primaryInteractor = secondaryInteractor;
        //             secondaryInteractor = null;
        //             Debug.Log("Secondary Interactor Reset and Primary Interactor Replaced");
        //         }
        //         else
        //         {
        //             attachTransform = null;
        //             primaryInteractor = null;
        //             Debug.Log("Primary Interactor Reset");
        //         }
        //     }

        //     base.OnHoverExited(args);
        // }
        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            if (leftAnchorTransform == null || rightAnchorTransform == null)
            {
                base.OnSelectEntering(args);
                return;
            }
            if (interactorsSelecting.Count > 0)
            {
                base.OnSelectEntering(args);
                return;
            }
            Debug.Log("OnSelectEntering " + interactorsSelecting.Count + " " + interactorsHovering.Count);

            if (args.interactorObject.transform.TryGetComponent(out ControllerAvatar controllerAvatar))
            {
                if (controllerAvatar.HandSide == HandSide.Left)
                {
                    attachTransform = leftAnchorTransform;
                }
                else
                {
                    attachTransform = rightAnchorTransform;
                }
                if (primaryInteractor == null)
                {
                    primaryInteractor = args.interactorObject.transform;
                }
            }

            base.OnSelectEntering(args);
        }
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            if (leftAnchorTransform == null || rightAnchorTransform == null)
            {
                base.OnSelectExited(args);
                return;
            }
            Debug.Log("OnSelectExited " + interactorsSelecting.Count + " " + interactorsHovering.Count);

            if (secondaryInteractor != null && args.interactorObject.transform == secondaryInteractor)
            {
                secondaryAttachTransform = null;
                secondaryInteractor = null;
                Debug.Log("Secondary Interactor Reset");
            }
            else if (primaryInteractor != null && args.interactorObject.transform == primaryInteractor)
            {
                if (secondaryInteractor != null)
                {
                    attachTransform = secondaryAttachTransform;
                    secondaryAttachTransform = null;
                    primaryInteractor = secondaryInteractor;
                    secondaryInteractor = null;
                    Debug.Log("Secondary Interactor Reset and Primary Interactor Replaced");
                }
                else
                {
                    attachTransform = null;
                    primaryInteractor = null;
                    Debug.Log("Primary Interactor Reset");
                }
            }

            base.OnSelectExited(args);
        }
        #endregion

        #region PRIVATE_METHODS
        private void OnItemGrabbed(SelectEnterEventArgs arg0)
        {
            if (shouldResetOnRelease && arg0.interactorObject.transform == primaryInteractor)
            {
                grabbedPosition = transform.position;
                grabbedRotation = transform.rotation;
            }

            if (arg0.interactorObject.transform == primaryInteractor)
            {
                OnGrabbed?.Invoke(this);
            }
            else if (arg0.interactorObject.transform == secondaryInteractor)
            {
                OnGrabbedSecondary?.Invoke(this);
            }
        }

        private void OnItemReleased(SelectExitEventArgs arg0)
        {
            if (shouldResetOnRelease && arg0.interactorObject.transform == primaryInteractor)
            {
                transform.position = grabbedPosition;
                transform.rotation = grabbedRotation;
            }

            if (arg0.interactorObject.transform == primaryInteractor)
            {
                OnReleased?.Invoke(this);
            }
            else if (arg0.interactorObject.transform == secondaryInteractor)
            {
                OnReleasedSecondary?.Invoke(this);
            }
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