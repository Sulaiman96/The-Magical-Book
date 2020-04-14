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

        public void Spawn(Transform handTransform, Animator animator)
        {
            if (weaponPrefab != null)
            {
                Instantiate(weaponPrefab, handTransform);
            }

            if (weaponOverride != null)
            {
                animator.runtimeAnimatorController = weaponOverride;
            }
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
