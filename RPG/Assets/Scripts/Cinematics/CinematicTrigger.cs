using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinmeatics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private bool alreadyPlayed = false;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && alreadyPlayed != true)
            {
                GetComponent<PlayableDirector>().Play();
                alreadyPlayed = true;
            }
            
        }
    }
}

