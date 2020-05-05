
using System;
using UnityEngine.UI;
using UnityEngine;

namespace RPG.Stats
{
    public class LevelDisplay: MonoBehaviour
    {
        BaseStats baseStats;
        private Text _text;

        private void Start()
        {
            _text = GetComponent<Text>();
        }

        private void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            _text.text = String.Format("{0:0}" , baseStats.GetCurrentLevel());
        }
    }
}