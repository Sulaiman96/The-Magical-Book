using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEditor;
using UnityEngine;

namespace RPG.Combat{
    public class Projectile : MonoBehaviour
    {
        public int projectileSpeed = 10;
        public bool isHoming = false;
        public GameObject hitEffect = null;
        Health target = null;
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

        public void SetTarget(Health target, float damage)
        {
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
            if (other.GetComponent<Health>() == target)
            {
                if (!target.IsDead())
                {
                    if(hitEffect != null) Instantiate(hitEffect, GetTargetLocation(), Quaternion.Euler(0,-45,0));
                    target.TakeDamage(damage);
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
