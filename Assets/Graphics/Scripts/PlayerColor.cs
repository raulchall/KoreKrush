using UnityEngine;

public class PlayerColor : MonoBehaviour {
	public MeshRenderer[] renderers;
	public int[] submeshIndex;
	public ParticleSystem[] particleSystems;
	public LensFlare[] flares;
	public Light[] lights;
	public Color color;

	void Start()
	{
		SetColors ();
	}

	public void SetColors()
	{
		for (int i = 0; i < renderers.Length; i++) {
			renderers [i].materials [submeshIndex [i]].SetColor("_GlowColor",color);
		}
		foreach (var item in lights) {
			item.color = color;
		}
		foreach (var item in flares) {
			item.color = color;
		}
		foreach (var item in particleSystems) {
			//ye to implement
		}
	}
}
