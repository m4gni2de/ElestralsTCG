using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using TMPro.EditorUtilities;
#endif
using UnityEngine;

namespace GlobalUtilities
{
    public static class SpriteFactory
    {

        public static Gradient CreateGradient(List<Element> elements)
        {

            Dictionary<Color, int> countByColor = new Dictionary<Color, int>();

            for (int i = 0; i < elements.Count; i++)
            {
                if (countByColor.ContainsKey(elements[i].ElementColor()))
                {
                    countByColor[elements[i].ElementColor()] += 1;
                }
                else
                {
                    countByColor.Add(elements[i].ElementColor(), 1);
                }
            }

            Gradient gradient = new Gradient();
            GradientColorKey[] colorKey;
            GradientAlphaKey[] alphaKey;

            colorKey = new GradientColorKey[countByColor.Count];
            alphaKey = new GradientAlphaKey[countByColor.Count];

            int count = 0;
            int index = 0;
            foreach (KeyValuePair<Color, int> col in countByColor)
            {
                alphaKey[index].alpha = 1.0f;
                alphaKey[index].time = 1.0f;

                colorKey[index].color = col.Key;
                colorKey[index].time = (float)count / (float)countByColor.Count;
                count += col.Value;
                index += 1;
            }

            gradient.SetKeys(colorKey, alphaKey);
            return gradient;
        }



        #region Texture Creating
        public static Texture2D GradientTexture(Gradient gradient, Texture2D baseTex)
        {
            int width = 256;
            int height = 1;
            byte[] data = baseTex.EncodeToPNG();

            width = baseTex.width;
            height = baseTex.height;
            // create texture
            Texture2D outputTex = new Texture2D(width, height, TextureFormat.ARGB32, true, true);
            outputTex.LoadImage(data);
            outputTex.wrapMode = TextureWrapMode.Clamp;
            outputTex.filterMode = FilterMode.Point;

            // draw texture
            for (int i = 0; i < width; i++)
            {
                for (int h = 0; h < height; h++)
                {
                    float r = outputTex.GetPixel(i, h).r;
                    float g = outputTex.GetPixel(i, h).g;
                    float b = outputTex.GetPixel(i, h).b;
                    float a = outputTex.GetPixel(i, h).a;


                    outputTex.SetPixel(i, h, gradient.Evaluate((float)i / (float)width));
                    Color newColor = new Color(outputTex.GetPixel(i, h).r * r, outputTex.GetPixel(i, h).g * g, outputTex.GetPixel(i, h).b * b, a);
                    if (a != 0)
                    {
                        outputTex.SetPixel(i, h, newColor);
                    }



                }
                //outputTex.SetPixel(i, 0, gradient.Evaluate((float)i / (float)width));
            }

            outputTex.Apply(false);

            return outputTex;
        }

        #endregion

    }

}

