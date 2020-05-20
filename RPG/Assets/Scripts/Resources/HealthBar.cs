using System;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace RPG.Resources
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health healthComponent = null;
        [SerializeField] private RectTransform foreground = null;
        [SerializeField] private Canvas rootCanvas = null;
        
        private void Update()
        {
            if (Mathf.Approximately(healthComponent.GetFraction(), 0)
            || Mathf.Approximately(healthComponent.GetFraction(), 1))
            {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;
            
            foreground.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);
        }
    }
}