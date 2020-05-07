using RPG.Core;
using RPG.Resources;
using UnityEngine;

namespace RPG.Combat    
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        public float weaponRange = 2f;
        public float weaponDamage = 25f;
        public float percentageBonus = 0f;
        public AnimatorOverrideController weaponOverride = null;
        public GameObject weaponPrefab = null;
        public bool isRightHandedWeapon = true;
        public Projectile projectile = null;

        private const string weaponName = "Weapon";
        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand); //destroy old weapon before picking up new one.
            
            if (weaponPrefab != null)
            {
                GameObject weapon = Instantiate(weaponPrefab, (isRightHandedWeapon ? rightHand : leftHand));
                weapon.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (weaponOverride != null)
            {
                animator.runtimeAnimatorController = weaponOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }

            if (oldWeapon == null) return;

            oldWeapon.name = "TO DESTROY"; //this is just so in case if player picks up two weapons quickly.
            Destroy(oldWeapon.gameObject);
        }
        public bool HasProjectile()
        {
            return projectile != null;
        }
        
        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile,
                (isRightHandedWeapon ? rightHand : leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target,instigator, calculatedDamage);
        }

        public float GetWeaponDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }
        public float GetWeaponRange()
        {
            return weaponRange;
        }
    }
}
