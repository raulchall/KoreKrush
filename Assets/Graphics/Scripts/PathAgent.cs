//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

[ExecuteInEditMode]
public class PathAgent : MonoBehaviour
{
	[Serializable]
	public class PathEvent
	{
		public GameObject obj;
		public float pathPosition;
		public bool setEnableTo;
		public PathEvent(GameObject target,float position, bool setTo)
		{
			obj = target;
			pathPosition = position;
			setEnableTo = setTo;
		}
	}

	public CinemachinePath path;
	public float Speed;
	public float pathAmount;
	public bool move;
	public List<PathEvent> events;
	private IEnumerator<PathEvent> eventsEnumerator;
	private PathEvent actualEvent;
	private bool made;
	private bool lastMoveNext = true;

	[HideInInspector]
	public List<PathEvent> additionalList = new List<PathEvent>();

	void Start()
	{
		pathAmount = 0;
		StartListening ();
	}

	void Update()
	{
		Check ();
		if (move)
			pathAmount += Speed * Time.deltaTime;
		transform.position = path.EvaluatePosition (pathAmount);
		transform.rotation = path.EvaluateOrientation (pathAmount);
	}

	void StartListening()
	{
		eventsEnumerator = events.GetEnumerator();
		eventsEnumerator.MoveNext ();
		actualEvent = eventsEnumerator.Current;
	}

	void Check(){
		if (lastMoveNext) {
			//Debug.Log ("ST_CHECK");
			if (!made && actualEvent != null) {
				if (actualEvent.pathPosition < pathAmount) {
					ExecuteEvent ();
					made = true;
				}
			}

			if (made) {
				lastMoveNext = eventsEnumerator.MoveNext ();
				if (lastMoveNext)
					actualEvent = eventsEnumerator.Current;
				made = false;
			}
			//Debug.Log ("END_CHECK");
		}
	}

	void ExecuteEvent()
	{
		actualEvent.obj.SetActive (actualEvent.setEnableTo);
	}
}
