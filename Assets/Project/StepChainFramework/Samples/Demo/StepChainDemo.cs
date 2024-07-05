using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChainFramework;
using Yudiz.XRStarter.Interactions;
public class StepChainDemo : MonoBehaviour
{
    public GameObject go1, go2,go3;
    public SnapZone snapZone;
    public AudioClip clipMain;
    public AudioClip repeatClip;

    Chain chain;
    private void Start()
    {
        chain = ChainManager.Get();

        chain.PlayAudio(clipMain)
            .Do(() => go1.SetActive(true))
            .AddSnapCondition(snapZone, go3)
            .Do(() => go2.SetActive(false))
            .PlayRepeatingReminder(repeatClip, 5, ()=> Debug.Log("Reminder Played"));

    }
}
