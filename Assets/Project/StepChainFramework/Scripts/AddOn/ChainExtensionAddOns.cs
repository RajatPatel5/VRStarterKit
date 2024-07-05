using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yudiz.XRStarter;
using Yudiz.XRStarter.Interactions;

namespace ChainFramework
{
    public static class ChainExtensionAddOns
    {
        public static Chain AddSnapCondition(this Chain chain, SnapZone snapZone, GameObject snapItem, Action completedAction = null)
        {
            chain.AddAction(new SnappedConditionalStep(snapZone, snapItem, completedAction));
            return chain;
        }

        public static Chain AddGrabCondition(this Chain chain, XRCustomGrabbable grabbable, Action completedAction = null)
        {
            chain.AddAction(new GrabbedConditionalStep(grabbable, completedAction));
            return chain;
        }

        public static Chain AddUnGrabCondition(this Chain chain, XRCustomGrabbable grabbable, Action completedAction = null)
        {
            chain.AddAction(new UnGrabbedConditionalStep(grabbable, completedAction));
            return chain;
        }
    }
}