using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1,99)]
        [SerializeField] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression = null;
        [SerializeField] private GameObject levelUpParticleEffect = null;
        [SerializeField] private bool shouldUseModifiers = false;

        public event Action onLevelUp;
        
        LazyValue<int> currentLevel;
        private Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateCurrentLevel);
        }
        private void Start()
        {
            currentLevel.ForceInit();
        }
        
        private void OnEnable()
        {
            if (experience != null)
            {
                //Not calling the method, but adding it to our delegate list.
                experience.onExperienceGained += UpdateLevel; 
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                //Not calling the method, but removing it from our delegate list.
                experience.onExperienceGained -= UpdateLevel; 
            }
        }
        
         
        private void UpdateLevel()
        {
            int newLevel = CalculateCurrentLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect,transform, false);
        }

        public float GetStat(Stats stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + (GetPercentageModifier(stat)/100));
        }

        private float GetBaseStat(Stats stat)
        {
            return progression.GetStat(stat, characterClass, GetCurrentLevel());
        }

        private float GetPercentageModifier(Stats stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifier(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }
        
        private float GetAdditiveModifier(Stats stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifier(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }

        public int GetCurrentLevel()
        {
            return currentLevel.value;
        }
        public int CalculateCurrentLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;

            float currentXP = experience.GetXP_Points();
            
            int penultimateLevel = progression.GetLevels(Stats.ExperienceToLevelUp, characterClass);
            for (int level = 1; level < penultimateLevel; level++)
            {
                float XPNeededToLevelUp = progression.GetStat(Stats.ExperienceToLevelUp, characterClass, level);
                if (XPNeededToLevelUp > currentXP)
                {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }
    }
}

