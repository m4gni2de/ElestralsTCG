using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FX
{
    public class BaseFX : MonoBehaviour
    {

        
        [HideInInspector] public Material ForceMaterial;
        [HideInInspector] public bool ActiveChange = true;

        public bool AutoSetMaterial = true;
        [HideInInspector] public int ShaderChange = 0;
        protected Material tempMaterial;
        protected Material defaultMaterial;
        protected Image CanvasImage;
        protected SpriteRenderer CanvasSpriteRenderer; [HideInInspector] public bool ActiveUpdate = true;

        protected string _shaderString = "";
        protected string shader
        {
            get
            {
                if (string.IsNullOrEmpty(_shaderString))
                {
                    _shaderString = GetShader();
                }
                return _shaderString; 
            }
        }

        #region Overrides
        protected virtual string GetShader()
        {
            return "Sprites/Default";
        }
        protected virtual void Show()
        {
            this.enabled = true;
        }
        protected virtual void Hide()
        {
            this.enabled = false;
        }
        #endregion

        void Awake()
        {
            SetRenderers();
        }
        protected void SetRenderers()
        {
            if (CanvasImage == null)
            {
                if (this.gameObject.GetComponent<Image>() != null) CanvasImage = this.gameObject.GetComponent<Image>();
            }

            if (CanvasSpriteRenderer == null)
            {
                if (this.gameObject.GetComponent<SpriteRenderer>() != null) CanvasSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
            }
        }

        void Start()
        {
            SetDefaultMaterial();
        }
        protected void SetDefaultMaterial()
        {
            ShaderChange = 0;

            if (CanvasSpriteRenderer != null)
            {
                if (CanvasSpriteRenderer.sharedMaterial.shader.name == "Sprites/Default")
                {
                    ForceMaterial.shader = Shader.Find(shader);
                    ForceMaterial.hideFlags = HideFlags.None;
                    CanvasSpriteRenderer.sharedMaterial = ForceMaterial;
                }
            }
            else if (CanvasImage != null)
            {
                Image img = CanvasImage;
                if (img.material == null)
                {
                    ForceMaterial.shader = Shader.Find(shader);
                    ForceMaterial.hideFlags = HideFlags.None;
                    CanvasImage.material = ForceMaterial;
                }
            }

            XUpdate();
        }
        // Start is called before the first frame update
        
        void Update()
        {
            if (ActiveUpdate) XUpdate();
        }


        protected virtual void DoSprite()
        {

        }
        protected virtual void DoImage()
        {

        }

        public void CallUpdate()
        {
            XUpdate();
        }
        void XUpdate()
        {

            SetRenderers();

            if ((ShaderChange == 0) && (ForceMaterial != null))
            {
                ShaderChange = 1;
                if (tempMaterial != null) DestroyImmediate(tempMaterial);

                if (CanvasSpriteRenderer != null)
                {
                    CanvasSpriteRenderer.sharedMaterial = ForceMaterial;
                }
                else if (CanvasImage != null)
                {
                    CanvasImage.material = ForceMaterial;
                }

                ForceMaterial.hideFlags = HideFlags.None;
                ForceMaterial.shader = Shader.Find(shader);

            }

            if ((ForceMaterial == null) && (ShaderChange == 1))
            {
                if (tempMaterial != null) DestroyImmediate(tempMaterial);
                tempMaterial = new Material(Shader.Find(shader));
                tempMaterial.hideFlags = HideFlags.None;

                if (CanvasSpriteRenderer != null)
                {
                    CanvasSpriteRenderer.sharedMaterial = tempMaterial;
                }

                else if (CanvasImage != null)
                {
                    CanvasImage.material = tempMaterial;
                }
                ShaderChange = 0;
            }

#if UNITY_EDITOR
            if (CanvasSpriteRenderer != null)
            {
                if (CanvasSpriteRenderer.sharedMaterial.shader.name == "Sprites/Default")
                {
                    ForceMaterial.shader = Shader.Find(shader);
                    ForceMaterial.hideFlags = HideFlags.None;
                    CanvasSpriteRenderer.sharedMaterial = ForceMaterial;
                }
            }
            else if (CanvasImage != null)
            {
                Image img = CanvasImage;
                if (img.material == null)
                {
                    ForceMaterial.shader = Shader.Find(shader);
                    ForceMaterial.hideFlags = HideFlags.None;
                    CanvasImage.material = ForceMaterial;
                }
            }
#endif
            if (ActiveChange)
            {

                if (CanvasSpriteRenderer != null) { DoSprite(); }

                else if (CanvasImage != null) { DoImage(); }
            }


        }

        void OnDestroy()
        {
            if ((Application.isPlaying == false) && (Application.isEditor == true))
            {

                if (tempMaterial != null) DestroyImmediate(tempMaterial);

                if (gameObject.activeSelf && defaultMaterial != null)
                {
                    if (CanvasSpriteRenderer != null)
                    {
                        CanvasSpriteRenderer.sharedMaterial = defaultMaterial;
                        CanvasSpriteRenderer.sharedMaterial.hideFlags = HideFlags.None;
                    }
                    else if (CanvasImage != null)
                    {
                        CanvasImage.material = defaultMaterial;
                        CanvasImage.material.hideFlags = HideFlags.None;
                    }
                }
            }
        }
        void OnDisable()
        {
            if (gameObject.activeSelf && defaultMaterial != null)
            {
                if (CanvasSpriteRenderer != null)
                {
                    CanvasSpriteRenderer.sharedMaterial = defaultMaterial;
                    CanvasSpriteRenderer.sharedMaterial.hideFlags = HideFlags.None;
                }
                else if (CanvasImage != null)
                {
                    CanvasImage.material = defaultMaterial;
                    CanvasImage.material.hideFlags = HideFlags.None;
                }
            }
        }
        void OnEnable()
        {
            if (defaultMaterial == null)
            {
                defaultMaterial = new Material(Shader.Find("Sprites/Default"));
            }

            if (ForceMaterial == null)
            {
                ActiveChange = true;
                tempMaterial = new Material(Shader.Find(shader));
                tempMaterial.hideFlags = HideFlags.None;

                if (CanvasSpriteRenderer != null)
                {
                    CanvasSpriteRenderer.sharedMaterial = tempMaterial;
                }
                else if (CanvasImage != null)
                {
                    CanvasImage.material = tempMaterial;
                }
            }
            else
            {
                ForceMaterial.shader = Shader.Find(shader);
                ForceMaterial.hideFlags = HideFlags.None;
                if (CanvasSpriteRenderer != null)
                {
                    CanvasSpriteRenderer.sharedMaterial = ForceMaterial;
                }
                else if (CanvasImage != null)
                {
                    CanvasImage.material = ForceMaterial;
                }
            }

        }
    }
}
