using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StepFramework;
using Yudiz.XRStarter.Interactions;

namespace MySimulation
{
    public class MySimulation : CustomSimulation
    {
        #region REFERENCES

        [Header("References")]
        public GameObject go1;
        public GameObject go2;
        public GameObject go3;
        public SnapZone snapZone;
        public AudioClip clipMain;
        public AudioClip repeatClip;
        #endregion
    }
}