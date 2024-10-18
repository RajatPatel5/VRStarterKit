using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class HandPoseAnimationTool : EditorWindow
{
    private GameObject handModelParent;
    private SkinnedMeshRenderer handRenderer;
    private Transform[] handBones;
    private string animationName = "NewHandPoseAnimation";
    private float animationDuration = 1.0f;
    private bool isLeftHand = true; // Choose left or right hand for prefab naming

    [MenuItem("Tools/Hand Pose Animation Tool")]
    public static void ShowWindow()
    {
        GetWindow<HandPoseAnimationTool>("Hand Pose Animation Tool");
    }

    private void OnGUI()
    {
        // Select the parent GameObject of the hand model
        handModelParent = (GameObject)EditorGUILayout.ObjectField("Hand Parent Object", handModelParent, typeof(GameObject), true);

        if (handModelParent != null)
        {
            handRenderer = handModelParent.GetComponentInChildren<SkinnedMeshRenderer>();

            if (handRenderer != null && GUILayout.Button("Load Hand Bones"))
            {
                LoadHandBones();
            }

            if (handBones != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Modify Bone Rotations & Positions", EditorStyles.boldLabel);

                // Display controls to modify bone transforms
                // for (int i = 0; i < handBones.Length; i++)
                // {
                //     if (handBones[i] != null)
                //     {
                //         EditorGUILayout.LabelField("Bone: " + handBones[i].name);

                //         // Bone rotation
                //         handBones[i].localRotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", handBones[i].localRotation.eulerAngles));

                //         // Bone position
                //         handBones[i].localPosition = EditorGUILayout.Vector3Field("Position", handBones[i].localPosition);
                //     }
                // }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Animation Settings", EditorStyles.boldLabel);
                animationName = EditorGUILayout.TextField("Animation Name", animationName);
                animationDuration = EditorGUILayout.FloatField("Animation Duration", animationDuration);

                // Option to choose left or right hand
                isLeftHand = EditorGUILayout.Toggle("Is Left Hand?", isLeftHand);

                if (GUILayout.Button("Create Animation Clip"))
                {
                    CreateAnimationClip();
                }

                if (GUILayout.Button("Save Hand Pose as Prefab"))
                {
                    SaveHandPoseAsPrefab();
                }
            }
        }
    }

    private void LoadHandBones()
    {
        if (handRenderer != null)
        {
            handBones = handRenderer.bones;
            Debug.Log("Hand bones loaded.");
        }
        else
        {
            Debug.LogWarning("No SkinnedMeshRenderer found in the parent object.");
        }
    }

    private string GetBonePath(Transform bone)
    {
        string bonePath = "";
        Transform current = bone;

        // Traverse upwards to build the full bone path, from the bone to the root
        while (current != handRenderer.rootBone && current != null)
        {
            bonePath = current.name + (bonePath == "" ? "" : "/" + bonePath);
            current = current.parent;
        }
        bonePath = current.name + (bonePath == "" ? "" : "/" + bonePath);

        return bonePath;
    }

    private void CreateAnimationClip()
    {
        if (handBones == null || handBones.Length == 0)
        {
            Debug.LogWarning("No bones loaded. Cannot create animation.");
            return;
        }

        // Create an animation clip
        AnimationClip animClip = new AnimationClip();
        animClip.legacy = false; // Ensure the clip is not legacy so it can be used in Mecanim

        for (int i = 0; i < handBones.Length; i++)
        {
            if (handBones[i] != null)
            {
                // Get the bone path considering the hierarchy from the root bone
                string bonePath = GetBonePath(handBones[i]);

                // Rotation curves
                AnimationCurve curveRotX = new AnimationCurve();
                AnimationCurve curveRotY = new AnimationCurve();
                AnimationCurve curveRotZ = new AnimationCurve();
                AnimationCurve curveRotW = new AnimationCurve();

                Quaternion localRotation = handBones[i].localRotation;

                // Add keyframes for the rotation (start and end)
                curveRotX.AddKey(0f, localRotation.x);
                curveRotY.AddKey(0f, localRotation.y);
                curveRotZ.AddKey(0f, localRotation.z);
                curveRotW.AddKey(0f, localRotation.w);

                curveRotX.AddKey(animationDuration, localRotation.x);
                curveRotY.AddKey(animationDuration, localRotation.y);
                curveRotZ.AddKey(animationDuration, localRotation.z);
                curveRotW.AddKey(animationDuration, localRotation.w);

                // Set rotation curves
                animClip.SetCurve(bonePath, typeof(Transform), "localRotation.x", curveRotX);
                animClip.SetCurve(bonePath, typeof(Transform), "localRotation.y", curveRotY);
                animClip.SetCurve(bonePath, typeof(Transform), "localRotation.z", curveRotZ);
                animClip.SetCurve(bonePath, typeof(Transform), "localRotation.w", curveRotW);

                // Position curves
                AnimationCurve curvePosX = new AnimationCurve();
                AnimationCurve curvePosY = new AnimationCurve();
                AnimationCurve curvePosZ = new AnimationCurve();

                Vector3 localPosition = handBones[i].localPosition;

                // Add keyframes for the position (start and end)
                curvePosX.AddKey(0f, localPosition.x);
                curvePosY.AddKey(0f, localPosition.y);
                curvePosZ.AddKey(0f, localPosition.z);

                curvePosX.AddKey(animationDuration, localPosition.x);
                curvePosY.AddKey(animationDuration, localPosition.y);
                curvePosZ.AddKey(animationDuration, localPosition.z);

                // Set position curves
                animClip.SetCurve(bonePath, typeof(Transform), "localPosition.x", curvePosX);
                animClip.SetCurve(bonePath, typeof(Transform), "localPosition.y", curvePosY);
                animClip.SetCurve(bonePath, typeof(Transform), "localPosition.z", curvePosZ);
            }
        }

        // Save the animation clip as an asset in the project
        string path = "Assets/Project/Models/Controller/Oculus Hands/Animations/" + animationName + ".anim";
        AssetDatabase.CreateAsset(animClip, path);
        AssetDatabase.SaveAssets();

        Debug.Log("Animation Clip Created: " + path);
    }

    private void SaveHandPoseAsPrefab()
    {
        if (handModelParent == null)
        {
            Debug.LogWarning("Please assign the parent GameObject of the hand model.");
            return;
        }

        // Create a unique prefab name based on whether it's the left or right hand
        string handType = isLeftHand ? "Left_hand_" : "Right_hand_";
        string prefabName = handType + animationName;
        string prefabPath = "Assets/Project/Models/Controller/Oculus Hands/Prefabs/" + prefabName + ".prefab";

        // Create a prefab from the parent GameObject of the hand model and save it
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(handModelParent, prefabPath);

        if (prefab != null)
        {
            Debug.Log("Hand Prefab Saved: " + prefabPath);
        }
        else
        {
            Debug.LogWarning("Failed to save hand prefab.");
        }
    }
}
