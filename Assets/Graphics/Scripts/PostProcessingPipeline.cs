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
	private RenderTexture _tempBuffer;
	private Camera cam;
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

		if (_tempBuffer != null) {
			DestroyImmediate (_tempBuffer);
		}
		_NormalsDisplay = new RenderTexture (width/2, height/2, 16);
		_Half = new RenderTexture( width/4, height/4, 16);
		_Vignette = new RenderTexture (512, 512, 16);
		_tempBuffer = new RenderTexture (width / 4, height / 4, 16);

		Shader.SetGlobalTexture ("_NormalsDisplay", _NormalsDisplay);
		Shader.SetGlobalTexture ("_Half", _Half);
		Shader.SetGlobalTexture ("_Vignette", _Vignette);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if (!init) {
			Graphics.Blit (src, _Vignette, pixelInfoRecorder);
			init = true;
		}
		Graphics.Blit (src, _Half);
		ItterativeBlur();
		Graphics.Blit(src,dst,ScreenDistortionMaterial);
	}
		
	void OnDisable()
	{
		DistortionTextureCamera.gameObject.SetActive (false);
	}

	public void ItterativeBlur()
	{
		for (int i = 0; i < 2; i++){
			Graphics.Blit (_Half, _tempBuffer, BoxBlur);
			Graphics.Blit (_tempBuffer, _Half, BoxBlur);
		}
	}

	void OnEnable()
	{
		cam = GetComponent<Camera> ();
		GenerateRenderTextures ();
		DistortionTextureCamera.gameObject.SetActive (true);
		EmptyCamera(DistortionTextureCamera,_NormalsDisplay);
		DistortionTextureCamera.SetReplacementShader (DisplayNormalsReplaceShader, "RenderType");
	}
}