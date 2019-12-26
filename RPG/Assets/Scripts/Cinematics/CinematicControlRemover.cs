using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinmeatics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        private GameObject player;
        private bool alreadyPlayed = false;
        void Start()
        {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
            player = GameObject.FindWithTag("Player");
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
            if (other.CompareTag("Player") && alreadyPlayed != true)
            {
                GetComponent<PlayableDirector>().Play();
                alreadyPlayed = true;
            }
            
        }
    }
}

