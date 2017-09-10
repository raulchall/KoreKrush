using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KoreKrush;

public class MeteorManager : MonoBehaviour {

	public MeteorAppear info;

	void Awake()
	{

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void Destroy()
	{
		
	}

	void OnCollision()
	{
		GetComponent<PathAgent> ().move = false;
	}
}
