using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elements;
using UnityEngine.UI;

namespace CardsUI.Stones
{
    public class TypeStone : MonoBehaviour
    {
        public enum Orientation
        {
            Center = 0,
            Left = 1,
            Right = 2,
            LeftCenter = 3,
            RightCenter = 4
            
        }

        #region Static Props
        public static float OffCenterAngle = 31f;
        #endregion

        public Orientation location = Orientation.Center;

        private ElementCode _element = ElementCode.None;
        public ElementCode element { get { return _element; } }

        public SpriteDisplay spriteDisplay;


        
        private void Awake()
        {
            
        }

        public void SetStone(int code)
        {
            Show();
            if (code < 0) { code = -1; }
            _element = (ElementCode)code;

            string spName = $"{element.ToString()}Symbol";

            SetTypeSprite();
            Sprite sp = AssetPipeline.ByKey<Sprite>(spName);
            spriteDisplay.SetSprite(sp);
            //TypeSymbolSp.sprite = AssetPipeline.ByKey<Sprite>(spName);
        }

        public void SetLargeStone(int code)
        {
            Show();
            if (code < 0) { code = -1; }
            _element = (ElementCode)code;

            string spName = $"{element.ToString()}Symbol_Large";

            spriteDisplay.SetSprite(AssetPipeline.ByKey<Sprite>(spName, CardFactory.DefaultSleeves));
            Show();
            //TypeSymbolSp.sprite = AssetPipeline.ByKey<Sprite>(spName);
        }

        private void SetTypeSprite()
        {
            switch (location)
            {
                case Orientation.Center:
                    spriteDisplay.gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                    //TypeSymbolSp.gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                    break;
                case Orientation.Right: case Orientation.RightCenter:
                    spriteDisplay.gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                    //TypeSymbolSp.gameObject.transform.rotation = new Quaternion(0f, -OffCenterAngle, 0f, 0f);
                    break;
                case Orientation.Left: case Orientation.LeftCenter:
                    spriteDisplay.gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                    //TypeSymbolSp.gameObject.transform.rotation = new Quaternion(0f, OffCenterAngle, 0f, 0f);
                    break;
                default:
                    spriteDisplay.gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                    //TypeSymbolSp.gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                    break;
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

