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

	public void DoneTutorialMemory()
	{
		string key = "Tutorial_Done_memory";
		PlayerPrefs.SetInt (key, 1);
	}

	public bool IsTutorialDoneMemory()
	{
		string key = "Tutorial_Done_memory";
		int rec = PlayerPrefs.GetInt (key, 0);
		return rec == 1;
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

