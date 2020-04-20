using System;
using UnityEngine;
using System.Collections.Generic;

namespace RPG.Core
{
    public class AfterEffectsCleaner : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (!_particleSystem.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}