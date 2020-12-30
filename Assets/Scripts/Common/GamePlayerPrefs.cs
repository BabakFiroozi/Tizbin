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

	

}

