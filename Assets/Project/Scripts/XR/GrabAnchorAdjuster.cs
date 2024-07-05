using NaughtyAttributes;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Yudiz.XRStarter
{
    [ExecuteInEditMode]
    public class GrabAnchorAdjuster : MonoBehaviour
    {
        private XRCustomGrabbable grabbable;
#if UNITY_EDITOR

        private void Awake()
        {
            grabbable = GetComponent<XRCustomGrabbable>();
        }
        #region EDITOR_TOOLS_METHODS
        private string handPosesFolderPath = "Assets/Project/Models/Controller/Oculus Hands/Prefabs/";
        private GameObject leftHandPreview;
        private GameObject rightHandPreview;
        [Button("SpawnLeftHandPreview")]
        public void SpawnLeftHandPreview()
        {
            Debug.Log("Spawning Left Checking Grab");
            if (grabbable == null)
                grabbable = GetComponent<XRCustomGrabbable>();
            Debug.Log("Spawning Left");

            if (leftHandPreview != null)
                return;

            if (grabbable.leftAnchorTransform == null)
            {
                GameObject leftAnchor = new GameObject("LeftHand_AnchorTransform");
                leftAnchor.transform.parent = transform;
                leftAnchor.transform.localPosition = Vector3.zero;
                leftAnchor.transform.localRotation = Quaternion.identity;
                grabbable.leftAnchorTransform = leftAnchor.transform;
            }

            grabbable.leftAnchorTransform.transform.localScale = Vector3.one;
            Vector3 globalScale = grabbable.leftAnchorTransform.transform.lossyScale;
            grabbable.leftAnchorTransform.transform.localScale = new Vector3(1 / globalScale.x, 1 / globalScale.y, 1 / globalScale.z);

            ControllerAvatar[] avatars = FindObjectsOfType<ControllerAvatar>();
            ControllerAvatar handAvatar = avatars.ToList().Find(x => x.HandSide == HandSide.Left);

            leftHandPreview = PrefabUtility.LoadPrefabContents(GetHandPosePrefab(HandSide.Right, grabbable.grabType));
            leftHandPreview.transform.parent = grabbable.leftAnchorTransform;
            leftHandPreview.transform.localPosition = handAvatar.HandTransform.localPosition;
            leftHandPreview.transform.localRotation = handAvatar.HandTransform.localRotation;

            leftHandPreview.transform.localScale = Vector3.one;
            globalScale = leftHandPreview.transform.lossyScale;
            leftHandPreview.transform.localScale = new Vector3(1 / globalScale.x, 1 / globalScale.y, 1 / globalScale.z);
        }
        [Button("RemoveLeftHandPreview")]
        public void RemoveLeftHandPreview()
        {
            if (leftHandPreview != null)
            {
                DestroyImmediate(leftHandPreview);
            }
        }
        [Button("SpawnRightHandPreview")]
        public void SpawnRightHandPreview()
        {
            Debug.Log("Spawning Right Checking Grab");
            if (grabbable == null)
                grabbable = GetComponent<XRCustomGrabbable>();
            Debug.Log("Spawning Right");

            if (rightHandPreview != null)
                return;

            if (grabbable.rightAnchorTransform == null)
            {
                GameObject leftAnchor = new GameObject("RightHand_AnchorTransform");
                leftAnchor.transform.parent = transform;
                leftAnchor.transform.localPosition = Vector3.zero;
                leftAnchor.transform.localRotation = Quaternion.identity;
                grabbable.rightAnchorTransform = leftAnchor.transform;
            }

            grabbable.rightAnchorTransform.transform.localScale = Vector3.one;
            Vector3 globalScale = grabbable.rightAnchorTransform.transform.lossyScale;
            grabbable.rightAnchorTransform.transform.localScale = new Vector3(1 / globalScale.x, 1 / globalScale.y, 1 / globalScale.z);

            ControllerAvatar[] avatars = FindObjectsOfType<ControllerAvatar>();
            ControllerAvatar handAvatar = avatars.ToList().Find(x => x.HandSide == HandSide.Right);


            rightHandPreview = PrefabUtility.LoadPrefabContents(GetHandPosePrefab(HandSide.Right, grabbable.grabType));
            rightHandPreview.transform.parent = grabbable.rightAnchorTransform;
            rightHandPreview.transform.localPosition = handAvatar.HandTransform.localPosition;
            rightHandPreview.transform.localRotation = handAvatar.HandTransform.localRotation;

            rightHandPreview.transform.localScale = Vector3.one;
            globalScale = rightHandPreview.transform.lossyScale;
            rightHandPreview.transform.localScale = new Vector3(1 / globalScale.x, 1 / globalScale.y, 1 / globalScale.z);
        }
        [Button("RemoveRightHandPreview")]
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
            if (handSide == HandSide.Left)
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
#endif
    }
}