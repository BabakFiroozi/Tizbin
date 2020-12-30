using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;


public static class GameSaveData
{
	public static void DoneTutorialSight()
	{
		string key = "Tutorial_Done_sight";
		PlayerPrefs.SetInt (key, 1);
	}

	public static bool IsTutorialDoneSight()
	{
		string key = "Tutorial_Done_sight";
		int rec = PlayerPrefs.GetInt (key, 0);
		return rec == 1;
	}

	public static void DoneTutorialMemory()
	{
		string key = "Tutorial_Done_memory";
		PlayerPrefs.SetInt (key, 1);
	}

	public static bool IsTutorialDoneMemory()
	{
		string key = "Tutorial_Done_memory";
		int rec = PlayerPrefs.GetInt (key, 0);
		return rec == 1;
	}

	public static void SetHint(int hint)
	{
		string key = "Hints_Count";
		PlayerPrefs.SetInt (key, hint);;
	}

	public static int GetHint()
	{
		string key = "Hints_Count";
		return PlayerPrefs.GetInt (key, 100);
	}

	public static void SetHelp(int help)
	{
		string key = "Helps_Count";
		PlayerPrefs.SetInt (key, help);;
	}

	public static int GetHelp()
	{
		string key = "Helps_Count";
		return PlayerPrefs.GetInt (key, 100);
	}
	
	
	public static bool GetBool(string key, bool def = false)
	{
		if (!PlayerPrefs.HasKey(key))
			return def;
		return PlayerPrefs.GetInt(key, 0) == 1;
	}

	public static void SetBool(string key, bool val)
	{
		PlayerPrefs.SetInt(key, val == true ? 1 : 0);
	}
	
	public static void SavePrefs()
	{
		PlayerPrefs.Save();
	}
	
	public static bool IsGameSoundOn()
	{
		string key = "Game_Sound_On";
		int on = PlayerPrefs.GetInt(key, 1);
		return on == 1;
	}

	public static void SetGameSoundOn(bool on)
	{
		string key = "Game_Sound_On";
		PlayerPrefs.SetInt(key, on ? 1 : 0);
	}

	public static bool IsGameMusicOn()
	{
		string key = "Game_Music_On";
		int on = PlayerPrefs.GetInt(key, 1);
		return on == 1;
	}

	public static void SetGameMusicOn(bool on)
	{
		string key = "Game_Music_On";
		PlayerPrefs.SetInt(key, on ? 1 : 0);
	}

	public static int GetSessionNumber()
	{
		return PlayerPrefs.GetInt("Session_Number", 0);
	}

	public static void IncreaseSessionNumber()
	{
		int num = GetSessionNumber() + 1;
		PlayerPrefs.SetInt("Session_Number", num);
	}

	public static bool IsGameVisited()
	{
		string key = "IsGameVisited";
		return GetBool(key, false);
	}

	public static void VisitGame()
	{
		string key = "IsGameVisited";
		SetBool(key, true);
	}

	public static void SetNextDailyEntranceDate(string data)
	{
		PlayerPrefs.SetString("DailyEntranceDate", data);
	}

	public static string GetLastDailyEntranceDate()
	{
		return PlayerPrefs.GetString("DailyEntranceDate", string.Empty);
	}

	public static void SetDailyEntranceNumber(int dayNum)
	{
		PlayerPrefs.SetInt("DailyEntranceNumber", dayNum);
		if (dayNum == 0)
		{
			ResetDailyFreeGuide();
			ResetDailyFreeCoin();
		}
	}
	
	static void ResetDailyFreeGuide()
	{
		for (int i = 0; i < 200; ++i)
		{
			int dayNum = i;
			string keyName = $"HasDailyFreeGuide_{dayNum}";
			if (PlayerPrefs.HasKey(keyName))
				PlayerPrefs.DeleteKey(keyName);
		}
	}
	
	static void ResetDailyFreeCoin()
	{
		for (int i = 0; i < 200; ++i)
		{
			int dayNum = i;
			string keyName = $"HasDailyFreeCoin_{dayNum}";
			if (PlayerPrefs.HasKey(keyName))
				PlayerPrefs.DeleteKey(keyName);
		}
	}

	/// <summary>
	/// Start from zero
	/// </summary>
	/// <returns></returns>
	public static int GetDailyEntranceNumber()
	{
		return PlayerPrefs.GetInt("DailyEntranceNumber", 0);
	}

	public static void IncreaseDailyEntranceNumber()
	{
		SetDailyEntranceNumber(GetDailyEntranceNumber() + 1);
	}
	
	public static void Reset()
	{
		PlayerPrefs.DeleteAll();
	}
}
