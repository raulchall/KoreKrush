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
		StartCoroutine ("DamageAtTime");
	}

	void OnEndCollision()
	{
		GetComponent<PathAgent> ().move = true;
		StopCoroutine ("DamageAtTime");
		Destroy (gameObject);
	}

	IEnumerator DamageAtTime()
	{
		while (true) {
			print (info.SpeedDamagePerTimeUnit);
			KoreKrush.Events.Logic.SpeedSubtracted (info.SpeedDamagePerTimeUnit);

			yield return new WaitForSeconds (info.SpeedDamageTimeUnit);
		}
	}

}
