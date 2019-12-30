using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            
            yield return fader.FadeOut(sceneFadeTime); //fades out the scene
            yield return SceneManager.LoadSceneAsync(sceneToLoad); //asynchronously loads the scene
            
            Portal newPortal = GetOtherPortal(); //find the portal in the new scene
            UpdatePlayerLocation(newPortal); //update the player's position and rotation to the new spawn point
            
            yield return new WaitForSeconds(sceneWaitTime); //Wait a few seconds so that the camera can stabilise
            yield return fader.FadeIn(sceneFadeTime); //fades in the scene 

            Destroy(this.gameObject);
        }

        private void UpdatePlayerLocation(Portal newPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.transform.position = newPortal.spawnPoint.position;
            player.transform.rotation = newPortal.spawnPoint.rotation;
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
