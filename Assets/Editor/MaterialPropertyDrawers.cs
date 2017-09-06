using UnityEngine;
using UnityEditor;

public class ConditionalVar : MaterialPropertyDrawer
{
	private string conditionalTo;

	public ConditionalVar(string conditionalTo)
	{
		this.conditionalTo = conditionalTo;
	}
	public override void OnGUI (Rect position, MaterialProperty prop, string label, MaterialEditor editor)
	{
		var conditionalValue = MaterialEditor.GetMaterialProperty (prop.targets,conditionalTo).floatValue;
		if (conditionalValue != 0) {
			editor.DefaultShaderProperty (prop,label);
		}
	}
}
