using UnityEngine;

[ExecuteInEditMode]
public class GlobalTextureRecorder : MonoBehaviour {

	[HideInInspector]
	[SerializeField]
	private Camera _camera;

	[Range(0,5)]
	public byte downResFactor = 3;

	public string globalTextureName = "_GlobalTex";

	void Start()
	{
		GenerateRT ();
	}

	void GenerateRT ()
	{
		_camera = GetComponent<Camera> ();

		if (_camera.targetTexture != null) {
			RenderTexture temp = _camera.targetTexture;
			_camera.targetTexture = null;
			DestroyImmediate(temp);
		}

		_camera.targetTexture = new RenderTexture (_camera.pixelWidth >> downResFactor, _camera.pixelHeight >> downResFactor, 16);
		_camera.targetTexture.filterMode = FilterMode.Bilinear;

		Shader.SetGlobalTexture (globalTextureName, _camera.targetTexture);
	}
}
