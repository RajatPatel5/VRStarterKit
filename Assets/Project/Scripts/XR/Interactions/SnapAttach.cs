using CommanTickManager;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Yudiz.XRStarter.Interactions
{
    public class SnapAttach : MonoBehaviour, ITick
    {
        [Header("Snapping Settings")]
        [SerializeField] private SnapAttachType snapAttachType;
        [SerializeField] private Transform snapAttachTransform;
        [SerializeField] private float distanceToDetech = 0.4f;
        [SerializeField] private bool lerpSnap;
        [ShowIf("lerpSnap")][SerializeField] private float snapSpeed = 10f;
        [ShowIf("lerpSnap")][SerializeField] private float alignSpeed = 50f;

        [Header("Other")]
        [SerializeField] private Transform snappableItem;

        [Foldout("Snap Events")] public UnityEvent<SnapAttach, Transform> OnSnapAttached;
        [Foldout("Snap Events")] public UnityEvent<SnapAttach, Transform> OnSnapDetached;
        [Foldout("Snap Events")] public UnityEvent<SnapAttach, Transform> OnSnappDetaching;
        [Foldout("Snap Events")] public UnityEvent<SnapAttach, Transform> OnSnappAttaching;

        private SnapState snapState;

        private Transform attachingItemParent;
        private Transform attachingTransform;

        #region UNITY_CALLBACKS
        private void Start()
        {
            snapState = SnapState.Idle;
            if (snapAttachTransform == null)
            {
                snapAttachTransform = transform;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("SnapTriggerEnter");
            if (snapState == SnapState.Idle)
            {
                switch (snapAttachType)
                {
                    case SnapAttachType.Hand:
                        if (other.TryGetComponent(out ControllerAvatar controllerAvatar))
                        {
                            attachingItemParent = controllerAvatar.transform;
                            attachingTransform = controllerAvatar.AvatarTransform;
                            ChangeSnapState(SnapState.Snapping);
                        }
                        break;
                    case SnapAttachType.Object:
                        if (other.TryGetComponent(out XRCustomGrabbable grabbable))
                        {
                            if (grabbable.snappableTransform == snappableItem)
                            {
                                attachingItemParent = grabbable.transform;
                                attachingTransform = grabbable.snappableTransform;
                                ChangeSnapState(SnapState.Snapping);
                            }
                        }
                        break;
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (snapState == SnapState.None)
            {
                switch (snapAttachType)
                {
                    case SnapAttachType.Hand:
                        if (other.TryGetComponent(out ControllerAvatar controllerAvatar))
                        {
                            if (controllerAvatar.AvatarTransform == attachingTransform)
                            {
                                ChangeSnapState(SnapState.Detaching);
                            }
                        }
                        break;
                    case SnapAttachType.Object:
                        if (other.TryGetComponent(out XRCustomGrabbable grabbable))
                        {
                            if (grabbable.snappableTransform == attachingTransform)
                            {
                                ChangeSnapState(SnapState.Detaching);
                            }
                        }
                        break;
                }
            }
        }
        #endregion

        #region PRIVATE_METHODS
        private void SnapItem(Transform snappingItem, Transform attachTransform, bool isSnapping)
        {
            if (lerpSnap && isSnapping)
            {
                snappingItem.position = Vector3.MoveTowards(snappingItem.position, attachTransform.position, Time.deltaTime * snapSpeed);
                snappingItem.rotation = Quaternion.RotateTowards(snappingItem.rotation, attachTransform.rotation, Time.deltaTime * alignSpeed);
            }
            else
            {
                snappingItem.position = attachTransform.position;
                snappingItem.rotation = attachTransform.rotation;
            }
        }
        private void DetachItem(Transform snappingItem, Transform attachTransform)
        {
            if (lerpSnap)
            {
                snappingItem.localPosition = Vector3.MoveTowards(snappingItem.localPosition, Vector3.zero, Time.deltaTime * snapSpeed);
                snappingItem.localRotation = Quaternion.RotateTowards(snappingItem.localRotation, Quaternion.identity, Time.deltaTime * alignSpeed);
            }
            else
            {
                snappingItem.localPosition = Vector3.zero;
                snappingItem.localRotation = Quaternion.identity;
            }
        }
        private void CheckForDetech(Transform snappingItem, Transform itemParent)
        {
            if (Vector3.Distance(snappingItem.position, itemParent.position) > distanceToDetech)
            {
                ChangeSnapState(SnapState.Detaching);
            }
        }
        private void ChangeSnapState(SnapState newState)
        {
            if (newState == snapState) { return; }

            ExitState(snapState);
            snapState = newState;
            EnterState(snapState);

        }
        private void ExitState(SnapState state)
        {
            switch (snapState)
            {
                case SnapState.Idle:
                    break;
                case SnapState.Snapping:
                    break;
                case SnapState.Snapped:
                    break;
                case SnapState.Detaching:
                    break;
                case SnapState.Detached:
                    attachingTransform = null;
                    attachingItemParent = null;
                    break;
            }
        }
        private void EnterState(SnapState state)
        {
            switch (snapState)
            {
                case SnapState.Idle:
                    ProcessingUpdate.Instance.Remove(this);
                    break;
                case SnapState.Snapping:
                    ProcessingUpdate.Instance.Add(this);
                    OnSnappAttaching?.Invoke(this, attachingTransform);
                    break;
                case SnapState.Snapped:
                    OnSnapAttached?.Invoke(this, attachingTransform);
                    break;
                case SnapState.Detaching:
                    OnSnappDetaching?.Invoke(this, attachingTransform);
                    break;
                case SnapState.Detached:
                    OnSnapDetached?.Invoke(this, attachingTransform);
                    ChangeSnapState(SnapState.Idle);
                    break;
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void ForceDetach()
        {
            ChangeSnapState(SnapState.Detaching);
        }
        public void Tick()
        {
            switch (snapState)
            {
                case SnapState.Snapping:
                    SnapItem(attachingTransform, snapAttachTransform, true);
                    if (Vector3.Distance(attachingTransform.position, snapAttachTransform.position) < 0.01f)
                    {
                        ChangeSnapState(SnapState.Snapped);
                    }
                    CheckForDetech(attachingTransform, attachingItemParent);
                    break;
                case SnapState.Snapped:
                    SnapItem(attachingTransform, snapAttachTransform, false);
                    CheckForDetech(attachingTransform, attachingItemParent);
                    break;
                case SnapState.Detaching:
                    DetachItem(attachingTransform, snapAttachTransform);
                    if (Vector3.Distance(attachingTransform.localPosition, Vector3.zero) < 0.1f)
                    {
                        attachingTransform.localPosition = Vector3.zero;
                        attachingTransform.localRotation = Quaternion.identity;
                        ChangeSnapState(SnapState.Detached);
                    }
                    break;
            }
        }
        public Transform GetAttachedReferenceTransform()
        {
            return attachingItemParent;
        }
        #endregion
    }

    public enum SnapAttachType
    {
        None,
        Hand,
        Object
    }
    public enum SnapState
    {
        None,
        Idle,
        Snapping,
        Snapped,
        Detaching,
        Detached
    }
}