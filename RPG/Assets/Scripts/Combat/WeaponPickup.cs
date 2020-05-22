using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Resources;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weaponConfig = null;
        [SerializeField] private float respawnTime = 5f;
        [SerializeField] private float restoreHealth = 0f;
        [SerializeField] private UnityEvent onPickUp;
        
        private Collider collider;

        private void Start()
        {
            collider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                onPickUp.Invoke();
                PickUp(other.gameObject);
            }
        }

        private void PickUp(GameObject subject)
        {
            if (weaponConfig != null)
            {
                subject.GetComponent<Fighter>().EquippingWeapon(weaponConfig);
            }

            if (restoreHealth > 0)
            {
                subject.GetComponent<Health>().Heal(restoreHealth);
            }
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow)
        {
            collider.enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            //You can change this into walk into range of the pickup, that way you can only pick it up if you're in range.
            if (Input.GetMouseButtonDown(0))
            {
                PickUp(callingController.gameObject);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.PickUp;
        }
    }
}