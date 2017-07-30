using UnityEngine;

[ExecuteInEditMode]
public class CustomEffectBlits : MonoBehaviour
{
	public Material effectMaterial;

	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		Graphics.Blit (src, dst, effectMaterial);
	}
}
