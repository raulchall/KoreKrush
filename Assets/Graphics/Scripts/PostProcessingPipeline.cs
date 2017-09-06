using UnityEngine;

[ExecuteInEditMode]
public class PostProcessingPipeline : MonoBehaviour
{
	public Material BoxBlur;
	public Material ScreenDistortionMaterial;
	public Shader DisplayNormalsReplaceShader;

	private RenderTexture _NormalsDisplay;
	private RenderTexture _Half;
	private RenderTexture _Vignette;
	public RenderTexture _MidLevel;
	private Camera cam;
	public Camera baseTextureCamera;
	public Camera DistortionTextureCamera;
	public Material pixelInfoRecorder;
	bool init = false;

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
		int width = cam.pixelWidth;//*cam.rect.width);
		int height = cam.pixelHeight;//cam.rect.height);

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

		if (_MidLevel != null) {
			baseTextureCamera.targetTexture = null;
			DestroyImmediate (_MidLevel);
		}

		if (width > 1080) {
			height = (int)(width *height / 1080.0f);
			width = 1080;
		}

		_NormalsDisplay = new RenderTexture (width, height, 16);
		_Half = new RenderTexture( width/4, height/4, 16);
		_Vignette = new RenderTexture (width, height, 16);
		_MidLevel = new RenderTexture (width, height, 16);
		//_MidLevel.antiAliasing = 4;
		Shader.SetGlobalTexture ("_NormalsDisplay", _NormalsDisplay);
		Shader.SetGlobalTexture ("_Half", _Half);
		Shader.SetGlobalTexture ("_Vignette", _Vignette);
		Shader.SetGlobalTexture ("_MidLevel", _MidLevel);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if (!init) {
			Graphics.Blit (src, _Vignette, pixelInfoRecorder);
			init = true;
		}
		EffectsHelper.ApplyIterativeEffect(_MidLevel,_Half,4,BoxBlur);
		Graphics.Blit(_MidLevel,dst,ScreenDistortionMaterial);
	}

	void OnDisable()
	{
		DistortionTextureCamera.gameObject.SetActive (false);
	}

	void OnEnable()
	{
		cam = GetComponent<Camera> ();
		GenerateRenderTextures ();
		DistortionTextureCamera.gameObject.SetActive (true);
		EmptyCamera(DistortionTextureCamera,_NormalsDisplay);
		EmptyCamera (baseTextureCamera, _MidLevel);
		DistortionTextureCamera.SetReplacementShader (DisplayNormalsReplaceShader, "RenderType");
	}
}