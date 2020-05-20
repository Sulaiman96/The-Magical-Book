using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        public enum DestinationIdentifier
        {
            A,B,C
        }
        public String sceneToLoad = "Outside";
        public Transform spawnPoint;
        public DestinationIdentifier destination;
        public float sceneFadeTime = 0.5f;
        public float sceneWaitTime = 0.5f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            DontDestroyOnLoad(this.gameObject);
            
            Fader fader = FindObjectOfType<Fader>();
            var playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;
            
            yield return fader.FadeOut(sceneFadeTime); //fades out the scene
            
            //Save current scene
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            wrapper.Save();
            
            yield return SceneManager.LoadSceneAsync(sceneToLoad); //asynchronously loads the scene
            var newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;
            //Load current scene
            wrapper.Load();
            
            Portal newPortal = GetOtherPortal(); //find the portal in the new scene
            UpdatePlayerLocation(newPortal); //update the player's position and rotation to the new spawn point
            
            wrapper.Save(); //checkpoint so when we close and open our game, it will start from the correct scene.
            
            yield return new WaitForSeconds(sceneWaitTime); //Wait a few seconds so that the camera can stabilise
            yield return fader.FadeIn(sceneFadeTime); //fades in the scene 
            newPlayerController.enabled = true;
            Destroy(this.gameObject);
        }

        private void UpdatePlayerLocation(Portal newPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = newPortal.spawnPoint.position;
            player.transform.rotation = newPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destination != this.destination) continue;
                return portal;
            }

            return null;
        }
    }
} 
