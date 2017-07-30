using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class counter : MonoBehaviour {
	private float time;
	private float Count;
	public PostProcessingPipeline pipe;
	public Text text;
	// Use this for initialization
	void Start () {
		time = Time.time;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Count++;
		if (Time.time > time + 1)
		{
			time = Time.time;
			Debug.Log (Count);
			text.text = "FPS " + Count;
			Count = 0;
		}
	}
	public void SetDudes()
	{
		pipe.enabled = !pipe.enabled;
	}
}
