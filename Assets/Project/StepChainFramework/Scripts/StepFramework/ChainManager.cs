using ChainFramework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ChainFramework
{
    public class ChainManager : MonoBehaviour
    {
        private static ChainManager _instance;
        private List<Chain> _chains = new List<Chain>();

        private void Awake()
        {
            if (_instance == null)
                Init();
        }

        public void Init()
        {
            _instance = this;

            DontDestroyOnLoad(gameObject);
        }
        public static Chain Get()
        {
            return _instance.CreateChain();
        }
        public static void Kill(Chain chain)
        {
            _instance.DestroyChain(chain);
        }

        public static void Clear(ref Chain chain)
        {
            chain?.Kill();
            chain = Get();
        }

        private void Update()
        {
            for (int i = 0; i < _chains.Count; ++i)
            {
                _chains[i].Run();
            }

            for (int i = 0; i < _chains.Count; ++i)
            {
                Chain chain = _chains[i];
                if (chain.IsFinished == false) continue;

                Kill(chain);
            }
        }

        private Chain CreateChain()
        {
            Chain newChain = new Chain();
            _chains.Add(newChain);

            return newChain;
        }
        private void DestroyChain(Chain chain)
        {
            if (_chains.Contains(chain))
            {
                _chains.Remove(chain);
            }
        }
    }
}