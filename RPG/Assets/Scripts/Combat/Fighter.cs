using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        #region Variables

        private Health target;
        public float timeBetweenAttacks = 1.35f;
        public Transform handTransform = null;
        public Weapon defaultWeapon = null;
        Mover mover;
        float timeSinceLastAttack = Mathf.Infinity;
        private ActionScheduler _actionScheduler;
        private Animator _animator;
        private Weapon currentWeapon = null;
        
        #endregion

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
            mover = GetComponent<Mover>();

            EquippingWeapon(defaultWeapon);
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
            currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            currentWeapon.Spawn(handTransform, animator);
        }

        //Animation Event
        void Hit()
        {
            if (target == null) return;
            target.TakeDamage(currentWeapon.getWeaponDamage());
        }

        //check to see if player is in range to target.
        private bool GetIsInRnage()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.getWeaponRange();
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
    }
}

