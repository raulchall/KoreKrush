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


	void OnCollision()
	{
		GetComponent<PathAgent> ().move = false;
		StartCoroutine ("DamageAtTime", info.SpeedDamageTimeUnit);
	}

	void OnEndCollision()
	{
		GetComponent<PathAgent> ().move = true;
		StopCoroutine ("DamageAtTime");
		Destroy (gameObject);
	}

	IEnumerator DamageAtTime(float time)
	{

		KoreKrush.Events.Logic.SpeedSubtracted (info.SpeedDamagePerTimeUnit);

		yield return new WaitForSeconds (time);
	}

}
