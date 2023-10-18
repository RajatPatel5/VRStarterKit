using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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

        #region UNITY_CALLBACKS
        protected override void Awake()
        {
            base.Awake();
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
        }

        private void OnItemReleased(SelectExitEventArgs arg0)
        {
            if (shouldResetOnRelease)
            {
                transform.position = grabbedPosition;
                transform.rotation = grabbedRotation;
            }
        }
        #endregion

        #region EDITOR_TOOLS_METHODS
        private string handPosesFolderPath = "Assets/Project/Models/Controller/Oculus Hands/Prefabs/";
        private GameObject leftHandPreview;
        private GameObject rightHandPreview;
        [ContextMenu("SpawnLeftHandPreview")]
        public void SpawnLeftHandPreview()
        {
            if (leftHandPreview != null)
                return;

            if (leftAnchorTransform == null)
            {
                GameObject leftAnchor = new GameObject("LeftHand_AnchorTransform");
                leftAnchor.transform.parent = transform;
                leftAnchor.transform.localPosition = Vector3.zero;
                leftAnchor.transform.localRotation = Quaternion.identity;
                leftAnchorTransform = leftAnchor.transform;
            }

            leftAnchorTransform.transform.localScale = Vector3.one;
            Vector3 globalScale = leftAnchorTransform.transform.lossyScale;
            leftAnchorTransform.transform.localScale = new Vector3(1 / globalScale.x, 1 / globalScale.y, 1 / globalScale.z);

            ControllerAvatar[] avatars = FindObjectsOfType<ControllerAvatar>();
            ControllerAvatar handAvatar = avatars.ToList().Find(x => x.HandSide == HandSide.Left);

            rightHandPreview = PrefabUtility.LoadPrefabContents(GetHandPosePrefab(HandSide.Right, grabType));
            rightHandPreview.transform.parent = rightAnchorTransform;
            leftHandPreview.transform.localPosition = handAvatar.AvatarTransform.localPosition;
            leftHandPreview.transform.localRotation = handAvatar.AvatarTransform.localRotation;
            
            leftHandPreview.transform.localScale = Vector3.one;
            globalScale = leftHandPreview.transform.lossyScale;
            leftHandPreview.transform.localScale = new Vector3(1 / globalScale.x, 1 / globalScale.y, 1 / globalScale.z);
        }
        [ContextMenu("RemoveLeftHandPreview")]
        public void RemoveLeftHandPreview()
        {
            if (leftHandPreview != null)
            {
                DestroyImmediate(leftHandPreview);
            }
        }
        [ContextMenu("SpawnRightHandPreview")]
        public void SpawnRightHandPreview()
        {
            if (rightHandPreview != null)
                return;

            if (rightAnchorTransform == null)
            {
                GameObject leftAnchor = new GameObject("RightHand_AnchorTransform");
                leftAnchor.transform.parent = transform;
                leftAnchor.transform.localPosition = Vector3.zero;
                leftAnchor.transform.localRotation = Quaternion.identity;
                rightAnchorTransform = leftAnchor.transform;
            }
            
            rightAnchorTransform.transform.localScale = Vector3.one;
            Vector3 globalScale = rightAnchorTransform.transform.lossyScale;
            rightAnchorTransform.transform.localScale = new Vector3(1 / globalScale.x, 1 / globalScale.y, 1 / globalScale.z);

            ControllerAvatar[] avatars = FindObjectsOfType<ControllerAvatar>();
            ControllerAvatar handAvatar = avatars.ToList().Find(x => x.HandSide == HandSide.Right);


            rightHandPreview = PrefabUtility.LoadPrefabContents(GetHandPosePrefab(HandSide.Right, grabType));
            rightHandPreview.transform.parent = rightAnchorTransform;
            rightHandPreview.transform.localPosition = handAvatar.AvatarTransform.localPosition;
            rightHandPreview.transform.localRotation = handAvatar.AvatarTransform.localRotation;
            
            rightHandPreview.transform.localScale = Vector3.one;
            globalScale = rightHandPreview.transform.lossyScale;
            rightHandPreview.transform.localScale = new Vector3(1 / globalScale.x, 1 / globalScale.y, 1 / globalScale.z);
        }
        [ContextMenu("RemoveRightHandPreview")]
        public void RemoveRightHandPreview()
        {
            if (rightHandPreview != null)
            {
                DestroyImmediate(rightHandPreview);
            }
        }
        public string GetHandPosePrefab(HandSide handSide, HandGrabType grabType)
        {
            string prefabName = string.Empty;
            if(handSide == HandSide.Left)
            {
                prefabName = "Left_hand_";
            }
            else
            {
                prefabName = "Right_hand_";
            }

            prefabName += grabType.ToString().ToLower();

            return handPosesFolderPath + prefabName + ".prefab";
        }
        #endregion
    }
}