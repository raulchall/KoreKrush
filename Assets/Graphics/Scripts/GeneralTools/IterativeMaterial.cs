using UnityEngine;

[ExecuteInEditMode]
public class IterativeMaterial : MonoBehaviour
{
	public Material material;

	[Range(0,3)]
	public byte iterations;

	[Range(0,3)]
	public int DownResFactor;

	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		int width = src.width >> DownResFactor;
		int height = src.height >> DownResFactor;

		RenderTexture rt = RenderTexture.GetTemporary (width, height);
		Graphics.Blit (src, rt);

		for (byte i = 0; i < iterations; i++) {
			RenderTexture rt2 = RenderTexture.GetTemporary(width,height);
			Graphics.Blit (rt, rt2, material);
			RenderTexture.ReleaseTemporary (rt);
			rt = rt2;
		}

		Graphics.Blit (rt, dst);
		RenderTexture.ReleaseTemporary (rt);
	}
}