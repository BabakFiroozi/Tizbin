﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class DataHelper
{
	static DataHelper s_instance = null;
	
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
	

	public int LevelsCount { get; private set; }

	public void CalcLevelsCount()
	{
		var levels = Resources.LoadAll<TextAsset>("Puzzles/");
		LevelsCount = levels.Length;
	}

	public static DataHelper Instance => s_instance ?? (s_instance = new DataHelper());

	public bool DailyEntrance { get; private set; }

	public bool DailyEntranceDisturbed => GameSaveData.GetDailyEntranceNumber() == 0;
	public bool InformDailyEntrance { get; private set; }
	public StoreNames StoreName { get; set; }

	public void CheckDailyEntrance(DateTime? nowDateTime)
	{
		DailyEntrance = false;

		if (nowDateTime != null)
		{
			var str = GameSaveData.GetLastDailyEntranceDate();
			if (str != string.Empty)
			{
				var jsonObj = JSONObject.Create(str);
				int year = (int) jsonObj["year"].i;
				int month = (int) jsonObj["month"].i;
				int day = (int) jsonObj["day"].i;
				var date = new DateTime(year, month, day, 0, 0, 0, 0);
				var diff = nowDateTime.Value - date;
				if (diff.TotalSeconds > 0)
				{
					if (diff.TotalHours < 24)
						DailyEntrance = true;
					else
						GameSaveData.SetDailyEntranceNumber(0);
				}
			}

			//Calculate next daily entrance
			{
				var today = nowDateTime.Value;
				int year = today.Year, month = today.Month, day = today.Day;
				if (day < DateTime.DaysInMonth(year, month))
				{
					day++;
				}
				else
				{
					day = 1;
					if (month < 12)
					{
						month++;
					}
					else
					{
						month = 1;
						year++;
					}
				}

				var jsonObj = JSONObject.Create();
				jsonObj.AddField("year", year);
				jsonObj.AddField("month", month);
				jsonObj.AddField("day", day);
				GameSaveData.SetNextDailyEntranceDate(jsonObj.Print());
			}
		}


		if (DailyEntrance)
			GameSaveData.IncreaseDailyEntranceNumber();

		InformDailyEntrance = DailyEntrance && GameSaveData.GetDailyEntranceNumber() > 0;
	}

	public bool IsFirstSession()
	{
		return GameSaveData.GetSessionNumber() == 1;
	}

	public DataHelper()
	{
	}
}
