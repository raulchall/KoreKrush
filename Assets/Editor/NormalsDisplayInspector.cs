using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 

public class NormalsDisplayInspector : CustomMaterialEditor
{
	protected override void CreateToggleList()
	{
		Toggles.Add(new FeatureToggle("Normals","normal","BUMP",""));
		//Toggles.Add(new FeatureToggle("Specular Enabled","specular","SPECULAR_ON","SPECULAR_OFF"));
		//Toggles.Add(new FeatureToggle("Fresnel Enabled","fresnel","FRESNEL_ON","FRESNEL_OFF"));
		//Toggles.Add(new FeatureToggle("Rim Light Enabled","rim","RIMLIGHT_ON","RIMLIGHT_OFF"));
	}
}