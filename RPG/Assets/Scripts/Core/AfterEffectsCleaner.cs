using System;
using UnityEngine;
using System.Collections.Generic;

namespace RPG.Core
{
    public class AfterEffectsCleaner : MonoBehaviour
    {
        private ParticleSystem _particleSystem;
        [SerializeField] GameObject targetToDestory = null;

        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (!_particleSystem.IsAlive())
            {
                if (targetToDestory != null)
                {
                    Destroy(targetToDestory);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}