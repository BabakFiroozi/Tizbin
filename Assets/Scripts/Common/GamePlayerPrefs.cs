using System;
using UnityEngine;

public class GamePlayerPrefs
{
	private GamePlayerPrefs ()
	{
	}

	static GamePlayerPrefs s_instance;
	public static GamePlayerPrefs Instance
	{
		get
		{
			if(s_instance == null)
				s_instance = new GamePlayerPrefs();
			return s_instance;
		}
	}

	public bool IsGameSoundOn()
	{
		return true;
	}

	public void SavePrefs()
	{
		PlayerPrefs.Save ();
	}

	public void DoneTutorialSight()
	{
		string key = "Tutorial_Done_sight";
		PlayerPrefs.SetInt (key, 1);
	}

	public bool IsTutorialDoneSight()
	{
		string key = "Tutorial_Done_sight";
		int rec = PlayerPrefs.GetInt (key, 0);
		return rec == 1;
	}

	public void SetPlayRecord(GameModes mode, int stage, int rec)
	{
		string key = mode.ToString () + stage.ToString () + "record";
		PlayerPrefs.SetInt (key, rec);
	}

	public int GetPlayRecord(GameModes mode, int stage)
	{
		string key = mode.ToString () + stage.ToString () + "record";
		int rec = PlayerPrefs.GetInt (key, 0);
		return rec;
	}

	public void SetPlayTime(GameModes mode, int stage, int sec)
	{
		string key = mode.ToString () + stage.ToString () + "time";
		PlayerPrefs.SetInt (key, sec);
	}

	public int GetPlayTime(GameModes mode, int stage)
	{
		string key = mode.ToString () + stage.ToString () + "time";
		int sec = PlayerPrefs.GetInt (key, 0);
		return sec;
	}

	public void UnlockStage(GameModes mode, int stage)
	{
		string key = mode.ToString () + stage.ToString () + "unlock";
		PlayerPrefs.SetInt (key, 1);
	}

	public bool IsStageUnlocked(GameModes mode, int stage)
	{
		string key = mode.ToString () + stage.ToString () + "unlock";
		return PlayerPrefs.GetInt (key, 0) == 1;
	}

	public void SetHint(int hint)
	{
		string key = "Hints_Count";
		PlayerPrefs.SetInt (key, hint);;
	}

	public int GetHint()
	{
		string key = "Hints_Count";
		return PlayerPrefs.GetInt (key, 100);
	}

	public void SetHelp(int help)
	{
		string key = "Helps_Count";
		PlayerPrefs.SetInt (key, help);;
	}

	public int GetHelp()
	{
		string key = "Helps_Count";
		return PlayerPrefs.GetInt (key, 100);
	}

}

