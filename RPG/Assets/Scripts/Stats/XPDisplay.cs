
using System;
using UnityEngine.UI;
using UnityEngine;

namespace RPG.Stats
{
    public class XPDisplay : MonoBehaviour
    {
        Experience experience;
        private Text _text;

        private void Start()
        {
            _text = GetComponent<Text>();
        }

        private void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            _text.text = String.Format("{0:0}" , experience.GetXP_Points());
        }
    }
}