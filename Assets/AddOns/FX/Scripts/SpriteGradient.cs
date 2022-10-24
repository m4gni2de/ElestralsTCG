using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

namespace FX
{
    [ExecuteInEditMode]
    [AddComponentMenu("FX/SpriteGradient")]
    public class SpriteGradient : BaseFX
    {
        [HideInInspector] public Color _Color1 = new Color(1f, 0f, 0f, 1f);
        [HideInInspector] public Color _Color2 = new Color(1f, 1f, 0f, 1f);
        [HideInInspector] public Color _Color3 = new Color(0f, 1f, 1f, 1f);
        [HideInInspector] public Color _Color4 = new Color(0f, 1f, 0f, 1f);
        [Range(0, 1)][HideInInspector] public float _Alpha = 1f;


        protected Color this[int pos]
        {
            get
            {
                if (pos == 0) { return _Color1; }
                if (pos == 1) { return _Color2; }
                if (pos == 2) { return _Color3; }
                if (pos == 3) { return _Color4; }
                return Color.clear;
            }
            set
            {
                if (pos == 0) { _Color1 = value; }
                if (pos == 1) { _Color2 = value; }
                if (pos == 2) { _Color3 = value; }
                if (pos == 3) { _Color4 = value; }
               
            }
        }
        public Color[] colors
        {
            get
            {
                Color[] cols = new Color[3];
                cols[0] = _Color1;
                cols[1] = _Color2;
                cols[2] = _Color3;
                cols[3] = _Color4;

                return cols;

            }
        }
        protected override string GetShader()
        {
            return "FX/SpriteGradient";
        }


        protected override void DoSprite()
        {
            CanvasSpriteRenderer.sharedMaterial.SetFloat("_Alpha", 1 - _Alpha);
            CanvasSpriteRenderer.sharedMaterial.SetColor("_Color1", _Color1);
            CanvasSpriteRenderer.sharedMaterial.SetColor("_Color2", _Color2);
            CanvasSpriteRenderer.sharedMaterial.SetColor("_Color3", _Color3);
            CanvasSpriteRenderer.sharedMaterial.SetColor("_Color4", _Color4);
        }

        protected override void DoImage()
        {
            CanvasImage.material.SetFloat("_Alpha", 1 - _Alpha);
            CanvasImage.material.SetColor("_Color1", _Color1);
            CanvasImage.material.SetColor("_Color2", _Color2);
            CanvasImage.material.SetColor("_Color3", _Color3);
            CanvasImage.material.SetColor("_Color4", _Color4);
        }
        // Start is called before the first frame update
       
        public void SetSingleColor(Color c1)
        {
            Color[] cols = new Color[4];
            cols[0] = c1;
            cols[1] = c1;
            cols[2] = c1;
            cols[3] = c1;
            SetColors(1f, cols);
        }
        public void SetDualGradient(Color c1, Color c2)
        {
            Color[] cols = new Color[4];
            cols[0] = c1;
            cols[1] = c2;
            cols[2] = c1;
            cols[3] = c2;
            SetColors(1f, cols);
        }

        public void SetColors(float alpha, Color[] cols)
        {
            if (cols.Length > 4) { throw new System.Exception("Can only input up to 4 colors."); }

            _Alpha = alpha;
            for (int i = 0; i < cols.Length; i++)
            {
                this[i] = cols[i];
            }

            Show();
            
        }


    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SpriteGradient)), CanEditMultipleObjects]
    public class SpriteGradientEditor : Editor
    {
        private SerializedObject m_object;

        public void OnEnable()
        {
            m_object = new SerializedObject(targets);
        }

        public override void OnInspectorGUI()
        {
            m_object.Update();

            DrawDefaultInspector();
            SpriteGradient fx = (SpriteGradient)target;


            EditorGUILayout.PropertyField(m_object.FindProperty("ActiveUpdate"), new GUIContent("Active Update", "Active Update, for animation / Animator only"));
            EditorGUILayout.PropertyField(m_object.FindProperty("ForceMaterial"), new GUIContent("Shared Material", "Use a unique material, reduce drastically the use of draw call"));

            if (fx.ForceMaterial == null)
            {
                fx.ActiveChange = true;
            }
            else
            {
                if (GUILayout.Button("Remove Shared Material"))
                {
                    fx.ForceMaterial = null;
                    fx.ShaderChange = 1;
                    fx._Color1 = new Color(1, 0, 0, 1);
                    fx._Color2 = new Color(0, 0, 1, 1);
                    fx._Color3 = new Color(0, 1, 0, 1);
                    fx._Color4 = new Color(0, 1, 1, 1);
                    fx.ActiveChange = true;
                    fx.CallUpdate();
                }
                EditorGUILayout.PropertyField(m_object.FindProperty("ActiveChange"), new GUIContent("Change Material Property", "Change The Material Property"));
            }

            if (fx.ActiveChange)
            {
                EditorGUILayout.BeginVertical("Box");
                Texture2D icone = Resources.Load("2dxfx-icon-corner-1") as Texture2D;

                EditorGUILayout.PropertyField(m_object.FindProperty("_Color1"), new GUIContent("Upper Left Color", icone, "Select the color from upper left"));
                icone = Resources.Load("2dxfx-icon-corner-2") as Texture2D;
                EditorGUILayout.PropertyField(m_object.FindProperty("_Color2"), new GUIContent("Upper Right Color", icone, "Select the color from upper right"));
                icone = Resources.Load("2dxfx-icon-corner-3") as Texture2D;
                EditorGUILayout.PropertyField(m_object.FindProperty("_Color3"), new GUIContent("Bottom Left Color", icone, "Select the color from Bottom left"));
                icone = Resources.Load("2dxfx-icon-corner-4") as Texture2D;
                EditorGUILayout.PropertyField(m_object.FindProperty("_Color4"), new GUIContent("Bottom Right Color", icone, "Select the color from Bottom right"));
                EditorGUILayout.BeginVertical("Box");
                icone = Resources.Load("2dxfx-icon-fade") as Texture2D;
                EditorGUILayout.PropertyField(m_object.FindProperty("_Alpha"), new GUIContent("Fading", icone, "Fade from nothing to showing"));
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();

                // PRESET FX
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField(new GUIContent("PRESET FX", "PRESET FX"));
                EditorGUILayout.BeginHorizontal("Box");
                Texture2D preview = Resources.Load("2dxfx-p-4g1") as Texture2D;

                if (GUILayout.Button(preview))
                {
                    fx._Color1 = new Color(1, 0, 0, 1);
                    fx._Color2 = new Color(0, 0, 1, 1);
                    fx._Color3 = new Color(0, 1, 0, 1);
                    fx._Color4 = new Color(0, 1, 1, 1);
                    fx._Alpha = 1;
                    m_object.ApplyModifiedProperties();
                    fx.CallUpdate();
                }

                preview = Resources.Load("2dxfx-p-4g2") as Texture2D;
                if (GUILayout.Button(preview))
                {
                    fx._Color1 = new Color(0, 1, 0.7f, 0);
                    fx._Color2 = new Color(0, 1, 0.7f, 0);
                    fx._Color3 = new Color(0, 1, 0.7f, 1);
                    fx._Color4 = new Color(0, 1, 0.7f, 1);
                    fx._Alpha = 1;
                    m_object.ApplyModifiedProperties();
                    fx.CallUpdate();
                }

                preview = Resources.Load("2dxfx-p-4g3") as Texture2D;
                if (GUILayout.Button(preview))
                {
                    fx._Color1 = new Color(1, 1, 0, 1);
                    fx._Color2 = new Color(1, 0.8f, 0, 0);
                    fx._Color3 = new Color(1, 0.6f, 0, 0);
                    fx._Color4 = new Color(1, 0.6f, 0, 0);
                    fx._Alpha = 1;
                    m_object.ApplyModifiedProperties();
                    fx.CallUpdate();
                }

                preview = Resources.Load("2dxfx-p-4g4") as Texture2D;
                if (GUILayout.Button(preview))
                {
                    fx._Color1 = new Color(1, 0, 0, 1);
                    fx._Color2 = new Color(1, 0, 0, 1);
                    fx._Color3 = new Color(1, 0, 0, 0);
                    fx._Color4 = new Color(1, 0, 0, 0);
                    fx._Alpha = 1;
                    m_object.ApplyModifiedProperties();
                    fx.CallUpdate();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

            }

            //fx.CheckForNullMaterial();
            m_object.ApplyModifiedProperties();

        }
    }
#endif
}



