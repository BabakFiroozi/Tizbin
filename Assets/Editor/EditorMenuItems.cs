using System;
using UnityEngine;
using UnityEditor;

public class EditorMenuItems
{

	[MenuItem("Tools/Clear Player Prefs")]
	public static void ClearePref()
	{
		if (EditorUtility.DisplayDialog ("Warning", "Are you sure to clear player prefrences?", "Yes", "No"))
			PlayerPrefs.DeleteAll ();
	}   
	
	[MenuItem("Assets/Create/Game Asset")]
	public static void CreateMyAsset()
	{
		var asset = ScriptableObject.CreateInstance<GameAsset>();
		AssetDatabase.CreateAsset(asset, "Assets/Resources/GameAsset.asset");
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}
}
