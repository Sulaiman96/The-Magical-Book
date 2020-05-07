using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Resources;
using RPG.Saving;
using RPG.Stats;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        #region Variables
        [SerializeField] float timeBetweenAttacks = 1.35f;
        [SerializeField] Transform rightHandTransform;
        [SerializeField] Transform leftHandTransform;
        [SerializeField] Weapon defaultWeapon;
        
        private Mover mover;
        private Health target;
        private float timeSinceLastAttack = Mathf.Infinity;
        private ActionScheduler _actionScheduler;
        private Animator _animator;
        LazyValue<Weapon> currentWeapon;
        #endregion

        private void Awake()
        {
            currentWeapon = new LazyValue<Weapon>(SetUpDefaultWeapon);
        }

        private Weapon SetUpDefaultWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
            mover = GetComponent<Mover>();
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;

            if (target.IsDead()) return;

            if (!GetIsInRnage())//if not in range and target exist, we move to target until we are in range then we stop. 
            {
                mover.MoveTo(target.transform.position, 1f);
            }
            else
            {
                mover.Cancel();
                AttackBehaviour();
            }
        }
            
        public void EquippingWeapon(Weapon weapon)
        {
            currentWeapon.value = weapon;
            AttachWeapon(weapon);
        }

        private void AttachWeapon(Weapon weapon)
        {
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        //Animation Event
        void Hit()
        {
            if (target == null) return;
            float damage = GetComponent<BaseStats>().GetStat(Stats.Stats.Damage);
            if (currentWeapon.value.HasProjectile())
            {
                currentWeapon.value.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage );
            }
            
        }

        void Shoot()
        {
            Hit();
        }

        //check to see if player is in range to target.
        private bool GetIsInRnage()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.value.GetWeaponRange();
        }

        #region Attacking
        public void Attack(GameObject combatTarget)
        {
            _actionScheduler.StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            mover.Cancel();
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttacking");
        }
        
        public IEnumerable<float> GetAdditiveModifier(Stats.Stats stat)
        {
            if (stat == Stats.Stats.Damage)
            {
                yield return currentWeapon.value.GetWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stats.Stats stat)
        {
            if (stat == Stats.Stats.Damage)
            {
                yield return currentWeapon.value.GetPercentageBonus();
            }
        }


        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            _animator.ResetTrigger("stopAttacking");
            _animator.SetTrigger("attack");
        }
        #endregion
        
        public object CaptureState()
        {
            return currentWeapon == null ? "Unarmed" : currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string) state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            EquippingWeapon(weapon);
        }

        public Health GetTarget()
        {
            return target;
        }

        
    }
}

