using RPG.Core;
using UnityEngine;

namespace RPG.Combat    
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        public float weaponRange = 2f;
        public float weaponDamage = 25f;
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
            if (rightHand == null) return;
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
        
        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)
        {
            Projectile projectileInstance = Instantiate(projectile,
                (isRightHandedWeapon ? rightHand : leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, weaponDamage);
        }

        public float getWeaponDamage()
        {
            return weaponDamage;
        }

        public float getWeaponRange()
        {
            return weaponRange;
        }
    }
}
