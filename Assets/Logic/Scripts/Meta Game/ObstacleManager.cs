using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KoreKrush;
public class ObstacleManager : MonoBehaviour {

	public Obstacle obstacle_info; 
	public RewardEvent info;

	// Use this for initialization
	protected void Start () {
		obstacle_info = info.Obj as Obstacle;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public virtual void OnCollision()
	{
		GetComponent<PathAgent> ().move = false;
		StartCoroutine ("DamageAtTime");
	}

	public virtual void OnEndCollision()
	{
		GetComponent<PathAgent> ().move = true;
		StopCoroutine ("DamageAtTime");
		Destroy (gameObject);
	}

	protected IEnumerator DamageAtTime()
	{
		while (true) {
			KoreKrush.Events.Logic.SpeedSubtract (obstacle_info.SpeedDamagePerTimeUnit);

			yield return new WaitForSeconds (obstacle_info.SpeedDamageTimeUnit);
		}
	}

}
