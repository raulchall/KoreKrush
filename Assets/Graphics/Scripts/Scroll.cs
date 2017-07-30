using UnityEngine;

public class Scroll : MonoBehaviour
{
	public Animator anim;
	public bool previus = false;
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update ()
	{
		bool getNow = Input.GetButton("Fire1");

		if (getNow != previus)
			anim.SetBool ("MAL", getNow);
		
		previus = getNow;
	}
}
