using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class HandPoseAnimationTool : EditorWindow
{
    private SkinnedMeshRenderer handRenderer;
    private Transform[] handBones;
    private string animationName = "NewHandPoseAnimation";
    private float animationDuration = 1.0f;

    [MenuItem("Tools/Hand Pose Animation Tool")]
    public static void ShowWindow()
    {
        GetWindow<HandPoseAnimationTool>("Hand Pose Animation Tool");
    }

    private void OnGUI()
    {
        // Select Skinned Mesh Renderer for the hands
        handRenderer = (SkinnedMeshRenderer)EditorGUILayout.ObjectField("Hand Model", handRenderer, typeof(SkinnedMeshRenderer), true);

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

            if (GUILayout.Button("Create Animation Clip"))
            {
                CreateAnimationClip();
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
            Debug.LogWarning("Please assign a Skinned Mesh Renderer.");
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
}
