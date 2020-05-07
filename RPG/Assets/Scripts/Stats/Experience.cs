using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] private float experiencePoints = 0;
        
        public event Action onExperienceGained;
        
        public void GainXP(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();
        }

        public float GetXP_Points()
        {
            return experiencePoints;
        }

        public object CaptureState()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float) state;
        }
    }
}