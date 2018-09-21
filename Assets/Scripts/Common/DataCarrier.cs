using System;
using UnityEngine;

public class DataCarrier
{
	public const int SCENE_CURRENT = -1;
	public const int SCENE_MAIN_MENU = 1;
	public const int SCENE_STAGE_MENU = 2;
	public const int SCENE_GAME = 3;

	public const int MAX_STAGES_COUNT = 20;

	public const float TIME_SCALE = 1.0f;

	public string TapsellAdId = "";
	public string TapsellAdIdInShop = "";
	public const string TAPSELL_ZONE_ID = "58eb65f84684650892df2220";

	static DataCarrier s_instance = null;

	public const int First_Hints = 999;

	public static DataCarrier Instance
	{
		get
		{
			if(s_instance == null)
			{
				s_instance = new DataCarrier();
				s_instance.Init();
			}
			return s_instance;
		}
	}

	JSONObject _options = null;

	StoreNames _storeName = StoreNames.Bazik;
	public StoreNames StoreName
	{
		get{return _storeName;}
		set{_storeName = value;}
	}

	public string GetString(string s)
	{
		var obj = _options.GetField ("Strings");
		return NBidi.NBidi.LogicalToVisual (obj.GetField (s).str);
	}

	public void Init()
	{
		var textAsset = Resources.Load<TextAsset> ("Options");
		_options = JSONObject.Create (textAsset.text);
	}

	public DataCarrier ()
	{
	}

	GameModes _gameMode = GameModes.Easy;
	public GameModes GameMode
	{
		get{return _gameMode;}
		set{_gameMode = value;}
	}

	int _selectedStage;
	public int SelectedStage
	{
		get{return _selectedStage;}
		set{_selectedStage = value;}
	}
}

