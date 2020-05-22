using RPG.Core;
using RPG.Resources;
using UnityEngine;

namespace RPG.Combat    
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        public float weaponRange = 2f;
        public float weaponDamage = 25f;
        public float percentageBonus = 0f;
        public AnimatorOverrideController weaponOverride = null;
        public Weapon weaponPrefab = null;
        public bool isRightHandedWeapon = true;
        public Projectile projectile = null;

        private const string weaponName = "Weapon";
        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand); //destroy old weapon before picking up new one.

            Weapon weapon = null;
            if (weaponPrefab != null)
            {
                weapon = Instantiate(weaponPrefab, (isRightHandedWeapon ? rightHand : leftHand));
                weapon.gameObject.name = weaponName;
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

            return weapon;
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
