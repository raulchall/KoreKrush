using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ReplaceShaderByRenderType : MonoBehaviour
{
	public Shader replaceShader;

	void OnEnable()
	{
		GetComponent<Camera>().SetReplacementShader(replaceShader, "RenderType");
	}
	void OnDisable()
	{
		GetComponent<Camera> ().ResetReplacementShader ();
	}
}