using UnityEngine;

[ExecuteInEditMode]
public class PostProcessingPipeline : MonoBehaviour
{
	public Material BoxBlur;
	public Material ScreenDistortionMaterial;
	public Shader DisplayNormalsReplaceShader;

	private RenderTexture _NormalsDisplay;
	private RenderTexture _Half;/*
	private RenderTexture _Quart;*/
	private Camera cam;
	public Camera DistortionTextureCamera;


	void Start()
	{
		Begin ();
	}

	void Begin()
	{
		cam = GetComponent <Camera> ();
		GenerateRenderTextures ();
		DistortionTextureCamera.gameObject.SetActive (true);
		EmptyCamera(DistortionTextureCamera,_NormalsDisplay);
		DistortionTextureCamera.SetReplacementShader(DisplayNormalsReplaceShader, "RenderType");
	}

	void End()
	{
		DistortionTextureCamera.gameObject.SetActive (false);
	/*	DestroyImmediate (_NormalsDisplay);
		DestroyImmediate (_Half);
		DestroyImmediate (_Quart);
	*/
	}

	void EmptyCamera(Camera cam, RenderTexture target)
	{
		if (cam.targetTexture != null) 
		{
			var tempTex = cam.targetTexture;
			cam.targetTexture = null;
			DestroyImmediate (tempTex);
		}
		cam.targetTexture = target;
	}

	void GenerateRenderTextures()
	{
		var width = cam.pixelWidth;
		var height = cam.pixelHeight;
		
		if (_NormalsDisplay != null) {
			DistortionTextureCamera.targetTexture = null;
			DestroyImmediate (_NormalsDisplay);
		}

		if (_Half != null) {
			DestroyImmediate (_Half);
		}
		/*
		if (_Quart != null) {
			DestroyImmediate (_Quart);
		}
*/
		_NormalsDisplay = new RenderTexture (width, height, 16);
		_Half = new RenderTexture( width>>3, height>>3, 16);
		//_Quart = new RenderTexture( width>>4, height>>4, 16);

		Shader.SetGlobalTexture ("_NormalsDisplay", _NormalsDisplay);
		Shader.SetGlobalTexture ("_Half", _Half);
		/*Shader.SetGlobalTexture ("_Quart", _Quart);*/
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		EffectsHelper.ApplyIterativeEffect(src,_Half,5,BoxBlur);
		Graphics.Blit(src,dst,ScreenDistortionMaterial);
	}

	void OnDisable()
	{
		End ();
	}
	void OnEnable()
	{
		Begin ();
	}
}