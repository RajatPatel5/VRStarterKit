using ChainFramework;
using StepFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySimulation
{
    [CreateAssetMenu(menuName = "Simulations/MySimulation/DemoStep1", fileName = "DemoStep1")]
    public class SimulationDemoStep : CustomSimulationStep<MySimulation>
    {
        public override void Init(StepStateMachine stateMachine, CustomSimulation customSim)
        {
            base.Init(stateMachine, customSim);
        }

        public override void OnStepEnter()
        {
            base.OnStepEnter();

            _mainStepChain.PlayAudio(_sim.clipMain)
           .Do(() => _sim.go1.SetActive(true))
           .AddSnapCondition(_sim.snapZone, _sim.go3)
           .Do(() => _sim.go2.SetActive(false))
           .PlayRepeatingReminder(_sim.repeatClip, 5, () => Debug.Log("Reminder Played"));
        }

        public override void OnInternalStepEnter(InternalStepsHandler stepHandler, int previousStepIndex, int enteredStepIndex)
        {
            base.OnInternalStepEnter(stepHandler, previousStepIndex, enteredStepIndex);
        }
        public override void OnInternalStepComplete(InternalStepsHandler stepHandler, int completedStepIndex)
        {
            base.OnInternalStepComplete(stepHandler, completedStepIndex);
        }

        public override void OnStepExit()
        {
            base.OnStepExit();
            Debug.Log("Exit Step 1");
        }
    }
}