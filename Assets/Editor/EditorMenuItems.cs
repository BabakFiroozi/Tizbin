using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EditorMenuItems
{

	[MenuItem("Tools/Clear Player Prefs")]
	public static void ClearePref()
	{
		if (EditorUtility.DisplayDialog ("Warning", "Are you sure to clear player prefrences?", "Yes", "No"))
			PlayerPrefs.DeleteAll ();
	}   
}
