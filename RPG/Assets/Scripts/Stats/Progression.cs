using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;
        Dictionary<CharacterClass, Dictionary<Stats, float[]>> lookupTable = null;
        
        public float GetStat(Stats stat,CharacterClass characterClass, int level)
        {
            BuildLookup();
            var statTable = lookupTable[characterClass];
            var levels = statTable[stat];
            if (levels.Length < level) return 0;
            return levels[level - 1];
        }

        public int GetLevels(Stats stat, CharacterClass characterClass)
        {
            BuildLookup();
            var statTable = lookupTable[characterClass];
            var levels = statTable[stat];
            return levels.Length;
        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;
            lookupTable = new Dictionary<CharacterClass, Dictionary<Stats, float[]>>();

            foreach (var progressionClass in characterClasses)
            {
                var statLookupTable = new Dictionary<Stats, float[]>();

                foreach (var progressionStat in progressionClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }
                
                lookupTable[progressionClass.characterClass] = statLookupTable;
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }
        
        [System.Serializable]
        class ProgressionStat 
        {
            public Stats stat;
            public float[] levels;
            
        }
    }
}

