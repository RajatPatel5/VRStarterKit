using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yudiz.XRStarter;

namespace ChainFramework
{
    public static class ChainExtensions
    {
        public static Chain Do(this Chain chain, Action action)
        {
            chain.AddAction(new DoActionStep(action));
            return chain;
        }

        public static Chain Wait(this Chain chain, float seconds)
        {
            chain.AddAction(new WaitStep(seconds));
            return chain;
        }
        public static Chain PlayAudio(this Chain chain, AudioClip clip)
        {
            chain.AddAction(new PlayAudioStep(clip));
            return chain;
        }
        public static Chain PlayRepeatingReminder(this Chain chain, AudioClip clip, int waitTimeInSeconds, Action repeatCallback = null)
        {
            chain.AddAction(new PlayRepeatAudioStep(clip, waitTimeInSeconds, repeatCallback));
            return chain;
        }

        public static Chain AddConditionalCompletion(this Chain chain, Action trueAction, Action falseAction, Func<bool> condition)
        {
            chain.AddAction(new ConditionalCompletionStep(trueAction, falseAction, condition));
            return chain;
        }
    }
}