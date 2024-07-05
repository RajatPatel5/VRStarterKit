using ChainFramework;
using StepFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySimulation
{
    [CreateAssetMenu(menuName = "Simulations/MySimulation/DemoStep2", fileName = "DemoStep2")]
    public class SimulationDemoStep2 : CustomSimulationStep<MySimulation>
    {

        private enum InternalStep
        {
            None = -1,

            Step1,
            Step2
        }

        public override void Init(StepStateMachine stateMachine, CustomSimulation customSim)
        {
            base.Init(stateMachine, customSim);

        }

        public override void OnStepEnter()
        {
            base.OnStepEnter();

            _internalStepHandler.Initialize(Enum.GetValues(typeof(InternalStep)));

            Debug.Log("Enter Step 2");

        }

        public override void OnInternalStepEnter(InternalStepsHandler stepHandler, int previousStepIndex, int enteredStepIndex)
        {
            base.OnInternalStepEnter(stepHandler, previousStepIndex, enteredStepIndex);

            switch ((InternalStep)enteredStepIndex)
            {
                case InternalStep.Step1:

                    _internalStepHandler.internalStepChain.Do(() => _sim.go1.SetActive(false))
                        .Wait(5)
                        .Do(() => _sim.go1.SetActive(false))
                        .Wait(5)
                        .Do(() => _sim.go1.SetActive(true))
                        .PlayRepeatingReminder(_sim.repeatClip, 5, () => Debug.Log("Reminder 3 Played"))
                        .Do(_internalStepHandler.CompleteInternalStep);

                    break;
                case InternalStep.Step2:

                    _internalStepHandler.internalStepChain.Do(() => _sim.go2.SetActive(false))
                        .Wait(5)
                        .Do(() => _sim.go2.SetActive(false))
                        .Wait(5)
                        .Do(() => _sim.go2.SetActive(true))
                        .PlayRepeatingReminder(_sim.repeatClip, 5, () => Debug.Log("Reminder 4 Played"))
                        .Do(_internalStepHandler.CompleteInternalStep);
                    break;
            }
        }
        public override void OnInternalStepComplete(InternalStepsHandler stepHandler, int completedStepIndex)
        {
            base.OnInternalStepComplete(stepHandler, completedStepIndex);

            switch ((InternalStep)completedStepIndex)
            {
                case InternalStep.Step1:

                    Debug.Log("Internal Step1 Completed");
                    break;

                case InternalStep.Step2:

                    Debug.Log("Internal Step2 Completed");
                    break;
            }
        }

        public override void OnStepExit()
        {
            base.OnStepExit();

            Debug.Log("Exit Step 2");
        }
    }
}