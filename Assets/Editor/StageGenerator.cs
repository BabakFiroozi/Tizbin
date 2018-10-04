using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class StageGenerator : EditorWindow {


	const int Window_Width = 300;
	const int Window_Height = 200;

	#region Menus
	[MenuItem("Tools/Stage Generator")]
	public static void ShowWindow()
	{
		var window = EditorWindow.GetWindowWithRect<StageGenerator> (new Rect (0, 0, Window_Width, Window_Height), true, "Stage Generator", true);
		//var window = EditorWindow.GetWindow<LevelEditor> ();
		window.Show ();
	}

	[MenuItem("Tools/Clear Player Prefs")]
	public static void ClearePref()
	{
		if (EditorUtility.DisplayDialog ("Warning", "Are you sure to clear player prefrences?", "Yes", "No"))
			PlayerPrefs.DeleteAll ();
	}
	#endregion


	GameModes _saveGameMode = GameModes.Easy;
	int _stagesCount = 50;
	int _maxSpritesCount = 20;
	float _glimpTime = 3;

	void Awake()
	{
	}

	void OnGUI()
	{
		_saveGameMode = (GameModes)EditorGUI.EnumPopup (new Rect (20, 30, 80, 20), (Enum)_saveGameMode);
		_stagesCount = EditorGUI.IntField (new Rect (110, 30, 50, 20), _stagesCount);
		_glimpTime = EditorGUI.FloatField (new Rect (170, 30, 50, 20), _glimpTime);

		_maxSpritesCount = EditorGUI.IntField (new Rect (120, 80, 80, 20), _maxSpritesCount);

		if(GUI.Button(new Rect (120, 120, 80, 20), "Generate"))
		{
			GenerateStage ();
		}
	}

	void GenerateStage()
	{
		int cellsCount = (int)_saveGameMode;

		JSONObject rootJsonObj = JSONObject.Create ();
		var stagesArr = JSONObject.arr;

		for(int i = 0; i < _stagesCount; ++i)
		{
			JSONObject stage = JSONObject.Create ();

			stage.AddField ("glimpTime", _glimpTime);

			List<int> cellsList = new List<int> ();
			for(int c = 0; c < cellsCount; ++c)
				cellsList.Add (c);
			List<int> randomsList = new List<int> ();
			do
			{
				int index = UnityEngine.Random.Range(0, cellsList.Count);
				randomsList.Add(cellsList[index]);
				cellsList.RemoveAt(index);
			}while(cellsList.Count > 0);
			JSONObject cellsArr = JSONObject.arr;
			foreach (var n in randomsList)
				cellsArr.Add (n);
			stage.AddField ("cells", cellsArr);

			List<int> spritesList = new List<int> ();
			for(int c = 0; c < _maxSpritesCount; ++c)
				spritesList.Add (c);
			randomsList.Clear ();
			do
			{
				int index = UnityEngine.Random.Range(0, spritesList.Count);
				randomsList.Add(spritesList[index]);
				spritesList.RemoveAt(index);
			}while(spritesList.Count != _maxSpritesCount - cellsCount / 2);

			JSONObject spritesArr = JSONObject.arr;
			foreach (var n in randomsList)
				spritesArr.Add (n);
			stage.AddField ("sprites", spritesArr);

			stagesArr.Add (stage);
		}

		rootJsonObj.AddField ("stages", stagesArr);

		string savePath = MakeSavePath ();
		File.WriteAllText (savePath, rootJsonObj.Print ());

		//Import asset again for updating file
		string filePath = "Assets/Resources/Levels/" +  _saveGameMode.ToString () + ".json";
		AssetDatabase.ImportAsset (filePath);
	}

	void OnDestroy()
	{
	}

	string MakeSavePath()
	{
		string path = Application.dataPath + "/Resources/Levels/" + _saveGameMode.ToString () + ".json";
		return path;
	}

}
