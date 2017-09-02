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
	public float Speed;
	[HideInInspector]
	public PathSection[] PathSections = new PathSection[0];
	public float MaxPathAmount{ get; private set;}

	private float minLimit;
	private float maxLimit;
	private int prevCamera;
	private int previousCamera;

	void Start()
	{
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

	}

	void Update()
	{
		Check ();
		if (move)
			pathAmount += Speed * Time.deltaTime;
		if (pathAmount < 0)
			pathAmount = 0;
		if (pathAmount > MaxPathAmount)
			pathAmount = MaxPathAmount;
		transform.position = path.EvaluatePosition (pathAmount);
		transform.rotation = path.EvaluateOrientation (pathAmount);
	}

	void Check()
	{
		if (pathAmount > maxLimit || pathAmount < minLimit) {
			bool camera_On = false;
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

					if (i > 1) {
						minLimit = PathSections [i - 1].PathPosition;
					}

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
