﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "LevelManager", menuName = "ScriptableObjects/Levels/LevelManager")]
public class LevelManager : ScriptableObject
{
	private string unlockedPref = "Unlocked";

	[SerializeField] private Level[] levels = null;
	public Level[] Levels => levels;

	[NonSerialized] public uint CurrentLevel = 0;
	[NonSerialized] public uint UnlockedLevel = 0;

	public void UnlockCurrentLevel()
	{
		UnlockedLevel = (uint)Mathf.Max(CurrentLevel, UnlockedLevel);
		PlayerPrefs.SetInt(unlockedPref, (int)UnlockedLevel);
	}

	public IEnumerable<Level> UnlockedLevels()
	{
		UnlockedLevel = Math.Max(UnlockedLevel, (uint) PlayerPrefs.GetInt(unlockedPref));
		for (int i = 0; i < levels.Length; i++)
		{
			if (i <= UnlockedLevel)
			{
				yield return levels[i];
			}
		}
	}
}
