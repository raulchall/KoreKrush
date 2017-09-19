using UnityEngine;

public static class EffectsHelper
{/*
	public static void ApplyIterativeEffect(RenderTexture tex_0, RenderTexture tex_1, int iterations, Material effectMaterial)
	{
		var width = tex_1.width;
		var height = tex_1.height;

		RenderTexture temp = RenderTexture.GetTemporary (width, height);
		Graphics.Blit (tex_0, temp);

		for (byte i = 0; i < iterations; i++) {
			RenderTexture temp2 = RenderTexture.GetTemporary (width, height);
			Graphics.Blit (temp, temp2, effectMaterial);
			RenderTexture.ReleaseTemporary (temp);
			temp = temp2;
		}
		Graphics.Blit (temp, tex_1);
		RenderTexture.ReleaseTemporary (temp);
	}*/
	/*
	public static void ItterativeEffect(RenderTexture src, RenderTexture dst, byte iterations, Material effect)
	{
		var width = dst.width;
		var height = dst.height;

		var tempDst = RenderTexture.GetTemporary (width, height);
		Graphics.Blit (src, tempDst);
		for (int i = 0; i < iterations; i++) {
			var temp = RenderTexture.GetTemporary (width,height);

		}
	}*/
}
