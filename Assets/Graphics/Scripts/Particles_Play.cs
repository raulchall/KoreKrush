using UnityEngine;

public class Particles_Play : MonoBehaviour
{
	public ParticleSystem[] particlesToPlay;
	public bool auto = true;
	//public Animator anim;

	void Start()
	{
		if (auto)
			particlesToPlay = GetComponentsInChildren<ParticleSystem> ();
	}

	public void ParticlesPlay()
	{
		foreach (var item in particlesToPlay) {
			item.Play ();
		}
	}

	public void ParticlesStop()
	{
		foreach (var item in particlesToPlay) {
			item.Stop ();
		}
	}
}
