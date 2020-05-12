using System;
using UnityEngine;
using System.Collections;
using GameDevTV.Utils;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine.Events;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float regenerationPercentage = 80f;
        [SerializeField] private TakeDamageEvent takeDamage;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
        }
        
        LazyValue<float> health;
        private bool isDead = false;

        private void Awake()
        {
            health = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stats.Stats.Health);
        }

        private void Start()
        {
            health.ForceInit();
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            health.value = Mathf.Max(health.value - damage, 0);
            takeDamage.Invoke(damage);
            if (health.value <= 0 && !isDead)
            {
                Die();
                GiveExperience(instigator);
            }
        }

        public float GetPercentageHealth()
        {
            return 100 * (health.value / GetComponent<BaseStats>().GetStat(Stats.Stats.Health));
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
            health.value = Mathf.Max(health.value, regenHealthPoints);
        }

        public bool IsDead()
        {
            return isDead;
        }

        public object CaptureState()
        {
            return health.value;
        }

        public void RestoreState(object state)
        {
            health.value = (float) state;
            if (health.value <= 0 && !isDead)
            {
                Die();
            }
        }
    }
}
