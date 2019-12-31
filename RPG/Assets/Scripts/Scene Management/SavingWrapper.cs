using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        private SavingSystem _savingSystem;
        private const string defaultSaveFile = "save";
        public float fadeInTime = 0.3f;
        
        IEnumerator Start()
        {
            _savingSystem = GetComponent<SavingSystem>();
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return _savingSystem.LoadLastScene(defaultSaveFile);
            yield return fader.FadeIn(fadeInTime);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
        }

        public void Save()
        {
            _savingSystem.Save(defaultSaveFile);
        }

        public void Load()
        {
            _savingSystem.Load(defaultSaveFile);
        }
    }
}

