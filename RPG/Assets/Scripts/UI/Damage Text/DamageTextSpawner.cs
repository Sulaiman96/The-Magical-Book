using UnityEngine;
using System;

namespace RPG.UI.Damage
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] private DamageText damageTextPrefab = null;
        
        public void Spawn(float damageAmount)
        {
            DamageText instance = Instantiate(damageTextPrefab, transform);
            instance.SetValue(damageAmount);
        }
    }
}