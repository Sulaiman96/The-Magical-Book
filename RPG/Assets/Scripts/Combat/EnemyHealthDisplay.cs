using System;
using RPG.Combat;
using RPG.Resources;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        private Fighter fighter;
        private Text _text;

        private void Start()
        {
            _text = GetComponent<Text>();
        }

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            if (fighter.GetTarget() == null)
            {
                _text.text = "N/A";
                return;
            }
            Health health = fighter.GetTarget();
            _text.text = String.Format("{0:0}%" ,health.GetPercentageHealth());
        }
    }
}

