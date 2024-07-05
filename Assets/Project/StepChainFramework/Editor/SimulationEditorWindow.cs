using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using StepFramework;

public class SimulationEditorWindow : EditorWindow
{
    private string simulationName = "NewSimulation";
    private List<string> majorSteps = new List<string> { "Step1" };
    private Dictionary<string, List<string>> internalSteps = new Dictionary<string, List<string>> { { "Step1", new List<string> { "InternalStep1" } } };
    private string simulationFolderName = "Simulations";
    private string projectFolderPath = "Assets/Project";
    private string projectScriptFolderPath = "Assets/Project/Scripts";

    [MenuItem("Tools/Simulation Creator")]
    public static void ShowWindow()
    {
        GetWindow<SimulationEditorWindow>("Simulation Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Simulation Settings", EditorStyles.boldLabel);

        simulationName = EditorGUILayout.TextField("Simulation Name", simulationName);

        GUILayout.Label("Major Steps", EditorStyles.boldLabel);
        for (int i = 0; i < majorSteps.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            majorSteps[i] = EditorGUILayout.TextField($"Step {i + 1}", majorSteps[i]);
            if (GUILayout.Button("Remove"))
            {
                internalSteps.Remove(majorSteps[i]);
                majorSteps.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();

            if (!internalSteps.ContainsKey(majorSteps[i]))
            {
                internalSteps[majorSteps[i]] = new List<string> { "InternalStep1" };
            }

            GUILayout.Label($"Internal Steps for {majorSteps[i]}", EditorStyles.boldLabel);
            for (int j = 0; j < internalSteps[majorSteps[i]].Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                internalSteps[majorSteps[i]][j] = EditorGUILayout.TextField($"Internal Step {j + 1}", internalSteps[majorSteps[i]][j]);
                if (GUILayout.Button("Remove"))
                {
                    internalSteps[majorSteps[i]].RemoveAt(j);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button($"Add Internal Step to {majorSteps[i]}"))
            {
                internalSteps[majorSteps[i]].Add($"InternalStep{internalSteps[majorSteps[i]].Count + 1}");
            }
        }

        if (GUILayout.Button("Add Major Step"))
        {
            majorSteps.Add($"Step{majorSteps.Count + 1}");
        }

        if (GUILayout.Button("Generate Simulation"))
        {
            GenerateSimulation();
        }

        if (GUILayout.Button("Generate Scriptable Objects and Setup Hierarchy"))
        {
            GenerateScriptableObjectsAndSetupHierarchy();
        }
    }

    private void GenerateSimulation()
    {
        string scriptsPath = $"{projectScriptFolderPath}/{simulationFolderName}";
        if (!AssetDatabase.IsValidFolder(scriptsPath))
        {
            AssetDatabase.CreateFolder(projectScriptFolderPath, simulationFolderName);
        }
        AssetDatabase.CreateFolder(scriptsPath, simulationName);
        scriptsPath = $"{projectScriptFolderPath}/{simulationFolderName}/{simulationName}";

        // Generate the main simulation script
        string simulationScriptPath = $"{scriptsPath}/{simulationName}.cs";
        string simulationScriptContent = GetSimulationScriptContent(simulationName);
        File.WriteAllText(simulationScriptPath, simulationScriptContent);
        AssetDatabase.ImportAsset(simulationScriptPath);

        // Generate the major step scripts
        foreach (var step in majorSteps)
        {
            string stepScriptPath = $"{scriptsPath}/{simulationName}{step}.cs";
            string stepScriptContent = GetStepScriptContent(simulationName, step, internalSteps[step]);
            File.WriteAllText(stepScriptPath, stepScriptContent);
            AssetDatabase.ImportAsset(stepScriptPath);
        }

        AssetDatabase.Refresh();
    }

    private void GenerateScriptableObjectsAndSetupHierarchy()
    {
        string assetsPath = $"{projectFolderPath}/{simulationFolderName}";
        if (!AssetDatabase.IsValidFolder(assetsPath))
        {
            AssetDatabase.CreateFolder(projectFolderPath, simulationFolderName);
            AssetDatabase.Refresh();
        }
        if (!AssetDatabase.IsValidFolder(assetsPath + "/" + simulationName))
        {
            AssetDatabase.CreateFolder(assetsPath, simulationName);
            AssetDatabase.Refresh();
        }
        assetsPath = $"{assetsPath}/{simulationName}";

        List<CustomStateMachineStep> stepScriptableObjects = new List<CustomStateMachineStep>();

        // Generate Scriptable Objects for each step
        foreach (var step in majorSteps)
        {
            string stepSOPath = $"{assetsPath}/{simulationName}{step}.asset";
            var stepSO = ScriptableObject.CreateInstance($"{simulationName}.{simulationName}{step}") as CustomStateMachineStep;
            AssetDatabase.CreateAsset(stepSO, stepSOPath);
            stepScriptableObjects.Add(stepSO);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Create objects in the hierarchy
        GameObject simulationObject = new GameObject(simulationName);
        System.Type simulationType = System.Type.GetType($"{simulationName}.{simulationName}, Assembly-CSharp");
        if (simulationType == null)
        {
            Debug.LogError($"Type {simulationName}.{simulationName} not found.");
            return;
        }
        CustomSimulation simulationComponent = simulationObject.AddComponent(simulationType) as CustomSimulation;

        GameObject stateMachineObject = new GameObject("StateMachine");
        StepStateMachine stateMachineComponent = stateMachineObject.AddComponent<StepStateMachine>();
        stateMachineComponent.customSimulation = simulationComponent;
        stateMachineComponent.steps = stepScriptableObjects;
    }

    private string GetSimulationScriptContent(string simulationName)
    {
        return $@"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StepFramework;

namespace {simulationName}
{{
    public class {simulationName} : CustomSimulation
    {{
        #region REFERENCES

        //Put All References Here

        #endregion
    }}
}}
";
    }

    private string GetStepScriptContent(string simulationName, string stepName, List<string> internalSteps)
    {
        string enumDefinitions = "";
        string internalStepCases = "";

        for (int i = 0; i < internalSteps.Count; i++)
        {
            enumDefinitions += $"{internalSteps[i]},\n";
            internalStepCases += $@"
                case InternalStep.{internalSteps[i]}:
                    // This is Example Usage
                    //_internalStepHandler.internalStepChain.Do(() => _sim.go1.SetActive(true)).Do(_internalStepHandler.CompleteInternalStep);
                    break;";
        }

        return $@"
using ChainFramework;
using StepFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace {simulationName}
{{
    [CreateAssetMenu(menuName = ""Simulations/{simulationName}/{stepName}"", fileName = ""{stepName}"")]
    public class {simulationName}{stepName} : CustomSimulationStep<{simulationName}>
    {{
        private enum InternalStep
        {{
            None = -1,
            {enumDefinitions}
        }}

        public override void Init(StepStateMachine stateMachine, CustomSimulation customSim)
        {{
            base.Init(stateMachine, customSim);
        }}

        public override void OnStepEnter()
        {{
            base.OnStepEnter();
            _internalStepHandler.Initialize(Enum.GetValues(typeof(InternalStep)));
        }}

        public override void OnInternalStepEnter(InternalStepsHandler stepHandler, int previousStepIndex, int enteredStepIndex)
        {{
            base.OnInternalStepEnter(stepHandler, previousStepIndex, enteredStepIndex);

            switch ((InternalStep)enteredStepIndex)
            {{
                {internalStepCases}
            }}
        }}

        public override void OnInternalStepComplete(InternalStepsHandler stepHandler, int completedStepIndex)
        {{
            base.OnInternalStepComplete(stepHandler, completedStepIndex);

            switch ((InternalStep)completedStepIndex)
            {{
                {internalStepCases}
            }}
        }}

        public override void OnStepExit()
        {{
            base.OnStepExit();
            Debug.Log(""Exit Step {stepName}"");
        }}
    }}
}}
";
    }
}
