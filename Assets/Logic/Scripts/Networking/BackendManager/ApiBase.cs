using UnityEngine;
using System.Collections;

public class ApiBase: MonoBehaviour
{
    protected BackendManager backendManager;
    void Awake()
	{
		backendManager = GetComponent<BackendManager>();
		if (backendManager == null)
		{
			Debug.LogWarning("BackendManager not found, disabling menu.");
			enabled = false;
		}
	}
}