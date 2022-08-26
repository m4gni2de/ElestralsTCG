using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardsUI.Glowing
{
    public class GlowObject : MonoBehaviour
    {
        public SpriteRenderer GlowSp;
        public SpriteRenderer MaskSp;
        public SpriteRenderer RainbowSp;

        public void Set(ElementCode code)
        {
            if (code == ElementCode.Any)
            {
                ToggleRainbow(true);
            }
            else
            {
                ToggleRainbow(false);
                GlowSp.color = GlowControls.GlowColor(code);
                MaskSp.color = GlowControls.MaskColor(code);
            }
           
        }

        private void ToggleRainbow(bool turnOn)
        {
            GlowSp.gameObject.SetActive(!turnOn);
            MaskSp.gameObject.SetActive(!turnOn);
            RainbowSp.gameObject.SetActive(turnOn);
        }

        public void SetBlank()
        {
            ToggleRainbow(false);
            GlowSp.color = Color.clear;
            MaskSp.color = Color.clear;

        }
    }
}

