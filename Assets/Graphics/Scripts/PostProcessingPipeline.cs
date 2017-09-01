using UnityEngine;

[ExecuteInEditMode]
public class PostProcessingPipeline : MonoBehaviour
{
	public Material BoxBlur;
	public Material ScreenDistortionMaterial;
	public Shader DisplayNormalsReplaceShader;

	private RenderTexture _NormalsDisplay;
	private RenderTexture _Half;
	public RenderTexture _Vignette;/*
private RenderTexture _Quart;*/
	private Camera cam;
	public Camera DistortionTextureCamera;
	public Material pixelInfoRecorder;

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
		if (_Vignette != null) {
			DestroyImmediate (_Vignette);
		}

		_NormalsDisplay = new RenderTexture (width, height, 16);
		_Half = new RenderTexture( width/6, height/6, 16);
		_Vignette = new RenderTexture (width, width, 16);

		Shader.SetGlobalTexture ("_NormalsDisplay", _NormalsDisplay);
		Shader.SetGlobalTexture ("_Half", _Half);
		Shader.SetGlobalTexture ("_Vignette", _Vignette);
	}
	bool init = false;
	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if (!init) {
			Graphics.Blit (src, _Vignette, pixelInfoRecorder);
			init = true;
		}
		EffectsHelper.ApplyIterativeEffect(src,_Half,4,BoxBlur);
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