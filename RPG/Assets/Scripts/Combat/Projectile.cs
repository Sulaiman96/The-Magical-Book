using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Resources;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] int projectileSpeed = 10;
        [SerializeField] bool isHoming = false;
        public GameObject hitEffect = null;
        Health target = null;
        [SerializeField] private UnityEvent onHit;
        
        private GameObject instigator = null;
        private float damage = 0f;

        private void Start()
        {
            transform.LookAt(GetTargetLocation());
        }

        private void Update()
        {
            if (target == null) return;
            if (isHoming && !target.IsDead())
            {
                transform.LookAt(GetTargetLocation());
            }
            transform.Translate(Vector3.forward * (projectileSpeed * Time.deltaTime));
            
        }

        public void SetTarget(Health target, GameObject instigator,  float damage)
        {
            
            this.instigator = instigator;
            this.damage = damage;
            this.target = target;
        }

        private Vector3 GetTargetLocation()
        {
            CapsuleCollider enemyCapsule = target.GetComponent<CapsuleCollider>();
            if (enemyCapsule == null)
            {
                return target.transform.position;
            }

            return target.transform.position + Vector3.up * (enemyCapsule.height * 3 / 4);
        }

        private void OnTriggerEnter(Collider other)
        {
            onHit.Invoke();
            if (other.GetComponent<Health>() == target)
            {
                if (!target.IsDead())
                {
                    if(hitEffect != null) Instantiate(hitEffect, GetTargetLocation(), Quaternion.Euler(0,-45,0));
                    target.TakeDamage(instigator, damage);
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(gameObject, 3f);
                }
            }
            else
            {
                return;
            }
        }
    }
}
