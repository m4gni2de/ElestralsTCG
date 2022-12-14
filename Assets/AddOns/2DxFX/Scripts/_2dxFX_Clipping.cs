//////////////////////////////////////////////
/// 2DxFX - 2D SPRITE FX - by VETASOFT 2020 //
//////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[AddComponentMenu("2DxFX/Standard/Clipping")]
[System.Serializable]
public class _2dxFX_Clipping : MonoBehaviour
{
    [HideInInspector] public Material ForceMaterial;
    [HideInInspector] public bool ActiveChange = true;
    private string shader = "2DxFX/Standard/Clipping";
    [HideInInspector] [Range(0, 1)] public float _Alpha = 1f;

    [HideInInspector] [Range(0f, 1f)] public float _ClipLeft = 0f;
    [HideInInspector] [Range(0f, 1f)] public float _ClipRight = 0f;
    [HideInInspector] [Range(0f, 1f)] public float _ClipUp = 0f;
    [HideInInspector] [Range(0f, 1f)] public float _ClipDown = 0f;

    [HideInInspector] public int ShaderChange = 0;
    Material tempMaterial;

    Material defaultMaterial;
    Image CanvasImage;
    SpriteRenderer CanvasSpriteRenderer;[HideInInspector] public bool ActiveUpdate = true;

    void Awake()
    {
        if (this.gameObject.GetComponent<Image>() != null) CanvasImage = this.gameObject.GetComponent<Image>();
        if (this.gameObject.GetComponent<SpriteRenderer>() != null) CanvasSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        ShaderChange = 0;
        XUpdate();
    }

    public void CallUpdate()
    {
        XUpdate();
    }


    void Update()
    {
        if (ActiveUpdate) XUpdate();
    }

    void XUpdate()
    {

        if (CanvasImage == null)
        {
            if (this.gameObject.GetComponent<Image>() != null) CanvasImage = this.gameObject.GetComponent<Image>();
        }
        if (CanvasSpriteRenderer == null)
        {
            if (this.gameObject.GetComponent<SpriteRenderer>() != null) CanvasSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        }
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
		string dfname = "";
		if(CanvasSpriteRenderer != null) dfname=CanvasSpriteRenderer.sharedMaterial.shader.name;
		if(CanvasImage != null) 
		{
			Image img = CanvasImage;
			if (img.material==null)	dfname="Sprites/Default";
		}
		if (dfname == "Sprites/Default")
		{
			ForceMaterial.shader=Shader.Find(shader);
			ForceMaterial.hideFlags = HideFlags.None;
			if(CanvasSpriteRenderer != null)
			{
				CanvasSpriteRenderer.sharedMaterial = ForceMaterial;
			}
			else if(CanvasImage != null)
			{
			Image img = CanvasImage;
				if (img.material==null)
				{
				CanvasImage.material = ForceMaterial;
				}
			}
		}
#endif
        if (ActiveChange)
        {
            if (CanvasSpriteRenderer != null)
            {
                CanvasSpriteRenderer.sharedMaterial.SetFloat("_Alpha", 1 - _Alpha);
                CanvasSpriteRenderer.sharedMaterial.SetFloat("_ClipLeft", 1 - _ClipLeft);
                CanvasSpriteRenderer.sharedMaterial.SetFloat("_ClipRight", 1 - _ClipRight);
                CanvasSpriteRenderer.sharedMaterial.SetFloat("_ClipUp", 1 - _ClipUp);
                CanvasSpriteRenderer.sharedMaterial.SetFloat("_ClipDown", 1 - _ClipDown);
            }
            else if (CanvasImage != null)
            {
                CanvasImage.material.SetFloat("_Alpha", 1 - _Alpha);
                CanvasImage.material.SetFloat("_ClipLeft", 1 - _ClipLeft);
                CanvasImage.material.SetFloat("_ClipRight", 1 - _ClipRight);
                CanvasImage.material.SetFloat("_ClipUp", 1 - _ClipUp);
                CanvasImage.material.SetFloat("_ClipDown", 1 - _ClipDown);

            }
        }

    }

    void OnDestroy()
    {
        if (this.gameObject.GetComponent<Image>() != null)
        {
            if (CanvasImage == null) CanvasImage = this.gameObject.GetComponent<Image>();
        }
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
        if (this.gameObject.GetComponent<Image>() != null)
        {
            if (CanvasImage == null) CanvasImage = this.gameObject.GetComponent<Image>();
        }
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
        if (this.gameObject.GetComponent<Image>() != null)
        {
            if (CanvasImage == null) CanvasImage = this.gameObject.GetComponent<Image>();
        }
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




#if UNITY_EDITOR
[CustomEditor(typeof(_2dxFX_Clipping)),CanEditMultipleObjects]
public class _2dxFX_Clipping_Editor : Editor
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
		
		_2dxFX_Clipping _2dxScript = (_2dxFX_Clipping)target;
	
		
        EditorGUILayout.PropertyField(m_object.FindProperty("ActiveUpdate"), new GUIContent("Active Update", "Active Update, for animation / Animator only"));
        EditorGUILayout.PropertyField(m_object.FindProperty("ForceMaterial"), new GUIContent("Shared Material", "Use a unique material, reduce drastically the use of draw call"));
		
		if (_2dxScript.ForceMaterial == null)
		{
			_2dxScript.ActiveChange = true;
		}
		else
		{
			if(GUILayout.Button("Remove Shared Material"))
			{
				_2dxScript.ForceMaterial= null;
				_2dxScript.ShaderChange = 1;
				_2dxScript.ActiveChange = true;
				_2dxScript.CallUpdate();
			}
		
			EditorGUILayout.PropertyField (m_object.FindProperty ("ActiveChange"), new GUIContent ("Change Material Property", "Change The Material Property"));
		}

		if (_2dxScript.ActiveChange)
		{

			EditorGUILayout.BeginVertical("Box");


			Texture2D icone = Resources.Load ("2dxfx-icon-clip_left") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_ClipLeft"), new GUIContent("Clipping Left", icone, "Clipping Left"));

			icone = Resources.Load ("2dxfx-icon-clip_right") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_ClipRight"), new GUIContent("Clipping Right", icone, "Clipping Right"));

			icone = Resources.Load ("2dxfx-icon-clip_up") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_ClipUp"), new GUIContent("Clipping Up", icone, "Clipping Up"));

			icone = Resources.Load ("2dxfx-icon-clip_down") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_ClipDown"), new GUIContent("Clipping Down", icone, "Clipping Down"));



			EditorGUILayout.BeginVertical("Box");

			icone = Resources.Load ("2dxfx-icon-fade") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_Alpha"), new GUIContent("Fading", icone, "Fade from nothing to showing"));

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndVertical();
	

		}
		
		m_object.ApplyModifiedProperties();
		
	}
}
#endif
