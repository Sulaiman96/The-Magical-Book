using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Resources
{
    public class HealthDisplay : MonoBehaviour
    {
        private Health _health;
        private Text _text;

        private void Start()
        {
            _text = GetComponent<Text>();
        }

        private void Awake()
        {
            _health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            _text.text = String.Format("{0:0}%" ,_health.GetPercentageHealth());
        }
    }
}

