using StepFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSimulationStep<T> : CustomStateMachineStep where T : CustomSimulation
{
    protected T _sim;

    public override void Init(StepStateMachine stateMachine, CustomSimulation customSim)
    {
        base.Init(stateMachine, customSim);
        _sim = (T)customSim;
    }
}
