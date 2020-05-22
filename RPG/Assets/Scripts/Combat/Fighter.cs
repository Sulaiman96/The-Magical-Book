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
        [SerializeField] WeaponConfig defaultWeaponConfig;
        
        private Mover mover;
        private Health target;
        private float timeSinceLastAttack = Mathf.Infinity;
        private ActionScheduler _actionScheduler;
        private Animator _animator;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;
        #endregion

        private void Awake()
        {
            
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
            mover = GetComponent<Mover>();
            currentWeaponConfig = defaultWeaponConfig;
            currentWeapon = new LazyValue<Weapon>(SetUpDefaultWeapon);
        }

        private Weapon SetUpDefaultWeapon()
        {
            return AttachWeapon(defaultWeaponConfig);
        }

        private void Start()
        {
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
            
        public void EquippingWeapon(WeaponConfig weaponConfig)
        {
            currentWeaponConfig = weaponConfig;
            currentWeapon.value = AttachWeapon(weaponConfig);
        }

        private Weapon AttachWeapon(WeaponConfig weaponConfig)
        {
            Animator animator = GetComponent<Animator>();
            return weaponConfig.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        //Animation Event
        void Hit()
        {
            if (target == null) return;
            
            float damage = GetComponent<BaseStats>().GetStat(Stats.Stats.Damage);
            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }
            
            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
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
            return Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.GetWeaponRange();
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
                yield return currentWeaponConfig.GetWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stats.Stats stat)
        {
            if (stat == Stats.Stats.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonus();
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
            return currentWeaponConfig == null ? "Unarmed" : currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string) state;
            WeaponConfig weaponConfig = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquippingWeapon(weaponConfig);
        }

        public Health GetTarget()
        {
            return target;
        }
    }
}

