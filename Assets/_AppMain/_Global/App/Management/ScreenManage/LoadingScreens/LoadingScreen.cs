using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppManagement.Loading
{
    public class LoadingScreen : MonoBehaviour
    {

       [SerializeField] private MultiImage sprites;

        public void Show()
        {
            gameObject.SetActive(true);
            for (int i = 0; i < sprites.images.Count; i++)
            {
                SpriteDisplay s = sprites.AtIndex(i);
                s.SetSortOrder((i * 2));
            }
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

