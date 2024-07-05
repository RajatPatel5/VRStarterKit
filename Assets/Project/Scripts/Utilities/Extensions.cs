using System;
using System.Collections;
using UnityEngine;

public static class Extensions 
{
    public static Coroutine Execute(this MonoBehaviour monoBehaviour, Action action, float time)
    {
        return monoBehaviour.StartCoroutine(DelayedAction(action, time));
    }


    static IEnumerator DelayedAction(Action action, float time)
    {
        yield return new WaitForSecondsRealtime(time);

        action();
    }
}
