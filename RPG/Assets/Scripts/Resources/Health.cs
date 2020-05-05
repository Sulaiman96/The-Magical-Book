using System;
using UnityEngine;
using System.Collections;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float regenerationPercentage = 80f;
        
        float health = -1f;
        private bool isDead = false;

        private void Start()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
            if (health < 0)
            {
                health = GetComponent<BaseStats>().GetStat(Stats.Stats.Health);
            }
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            print(gameObject.name + "took damage: " + damage);
            
            health = Mathf.Max(health - damage, 0);
            if (health <= 0 && !isDead)
            {
                Die();
                GiveExperience(instigator);
            }
        }

        public float GetPercentageHealth()
        {
            return 100 * (health / GetComponent<BaseStats>().GetStat(Stats.Stats.Health));
        }

        private void Die()
        {
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void GiveExperience(GameObject instigator)
        {
            Experience experience= instigator.GetComponent<Experience>();
            if (experience != null)
            {
                experience.GainXP(GetComponent<BaseStats>().GetStat(Stats.Stats.ExperiencePoints));
            }
        }
        
        private void RegenerateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stats.Stats.Health) * regenerationPercentage / 100;
            health = Mathf.Max(health, regenHealthPoints);
        }

        public bool IsDead()
        {
            return isDead;
        }

        public object CaptureState()
        {
            return health;
        }

        public void RestoreState(object state)
        {
            health = (float) state;
            if (health <= 0 && !isDead)
            {
                Die();
            }
        }
    }
}
