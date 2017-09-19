using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

[ExecuteInEditMode]
public class PathAgent : MonoBehaviour
{
	[Serializable]
	public class PathSection
	{
		public GameObject CameraObj;
		public float PathPosition;
		public PathSection(GameObject cameraObj,float pathPosition)
		{
			CameraObj = cameraObj;
			PathPosition = pathPosition;
		}
	}

	public CinemachinePath path;
	public float pathAmount;
	public float initialValue;
	public bool move;
	public float pathChunkLength;
	[HideInInspector]
	public PathSection[] PathSections = new PathSection[0];
	public float MaxPathAmount{ get; private set;}

	private float minLimit;
	private float maxLimit;
	private int prevCamera;
	private int previousCamera;
	public float updateRate = 0.3f;
	private float time;
	private Vector3 toPosition;
	//private Quaternion toRotation;
	public float maxSpeed = 0.1f;
	public float maxRotation = 0.1f;

	void Start()
	{
		time = Time.time;
		Innit ();
	}

	void Innit()
	{
		previousCamera = 0;
		pathAmount = initialValue;
		MaxPathAmount = path.MaxPos;
		maxLimit = 0;
		minLimit = MaxPathAmount;
		foreach (var item in PathSections) {
			item.CameraObj.SetActive (false);

		}
		//Vector3 position = path.EvaluatePosition (pathAmount);
		//Quaternion rotation = path.EvaluateOrientation (pathAmount);
		pathAmount += pathChunkLength;
		toPosition = path.EvaluatePosition (pathAmount);
		//toRotation = path.EvaluateOrientation (pathAmount);
		GetCorrectCamera ();
	}
	public float maxDistance = 0.025f;
	void FixedUpdate()
	{
		if (Time.time > time + updateRate) {
			//time = Time.time;
			//if (Vector3.Distance (transform.position, toPosition) <= maxDistance) {
			//	pathAmount += pathChunkLength;
			//	toPosition = path.EvaluatePosition (pathAmount);
			//	//toRotation = path.EvaluateOrientation (pathAmount);
			GetCorrectCamera();
			}
	}

	void LateUpdate()
	{
		pathAmount += maxSpeed*Time.deltaTime;

		if (pathAmount < 0)
			pathAmount = 0;
		if (pathAmount > MaxPathAmount)
			pathAmount = MaxPathAmount;
		
		transform.position = path.EvaluatePosition (pathAmount);
		//transform.position = Vector3.MoveTowards (transform.position, toPosition, maxSpeed);
		//transform.rotation = Quaternion.RotateTowards (transform.rotation, toRotation, maxRotation*Time.deltaTime);
		transform.rotation = path.EvaluateOrientation(pathAmount);
	}

	void GetCorrectCamera()
	{
		if (pathAmount > maxLimit || pathAmount < minLimit)
		{
			//bool camera_On = false;
			var length = PathSections.Length;
			maxLimit = MaxPathAmount;
			minLimit = 0;
			bool found = false;

			for (int i = 0; i < length; i++)
			{
				var temp = PathSections [i];
				if (temp.PathPosition > pathAmount)
				{
					maxLimit = temp.PathPosition;

					if (i > 1)
						minLimit = PathSections [i - 1].PathPosition;

					ChangeCameras (i > 0 ? i - 1 : 0);

					found = true;
					break;
				}
			}
			if (!found)
				ChangeCameras (length - 1);
		}
	}

	void ChangeCameras(int cameraIndex)
	{
		PathSections [previousCamera].CameraObj.SetActive (false);
		PathSections [cameraIndex].CameraObj.SetActive (true);
		previousCamera = cameraIndex;
	}
}
