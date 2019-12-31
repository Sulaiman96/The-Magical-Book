using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Saving;

namespace RPG.Cinmeatics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        private bool alreadyPlayed = false;
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
        }
    }
}

