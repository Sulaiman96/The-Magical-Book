using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Saving;

namespace RPG.Cinmeatics
{
    public class CinematicControlRemover : MonoBehaviour, ISaveable
    {
        private GameObject player;
        private bool alreadyPlayed = false;
        void Awake()
        {
            player = GameObject.FindWithTag("Player");
        }

        private void OnEnable()
        {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
        }

        private void OnDisable()
        {
            GetComponent<PlayableDirector>().played -= DisableControl;
            GetComponent<PlayableDirector>().stopped -= EnableControl;
        }

        void DisableControl(PlayableDirector pd)
        {
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }

        void EnableControl(PlayableDirector pd)
        {
            player.GetComponent<PlayerController>().enabled = true;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || alreadyPlayed == true) return;
            GetComponent<PlayableDirector>().Play();
            alreadyPlayed = true;
        }

        public object CaptureState()
        {
            return alreadyPlayed;
        }

        public void RestoreState(object state)
        {
            alreadyPlayed = (bool) state;
            if (alreadyPlayed == true) Debug.Log("Yup, cinematic already played");
        }
    }
}

