using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followDirection : MonoBehaviour
{
	private Vector3 position;
	private Vector3 prevPosition;
	public Transform vector;
	private Vector3 up = Vector3.up;
	public float rotation;
	// Use this for initialization
	void Start () {
		prevPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		position = transform.position;
		vector.rotation = Quaternion.RotateTowards (vector.rotation,Quaternion.LookRotation(position-prevPosition,up),rotation * Time.deltaTime);
		prevPosition = position;
	}
}
