using System;
using UnityEngine;

[CreateAssetMenu (fileName = "LevelManager", menuName = "ScriptableObjects/Levels/LevelManager")]
public class LevelManager : ScriptableObject
{
	[SerializeField] private Level[] levels = null;
	public Level[] Levels => levels;

	public uint CurrentLevel = 0;
	public uint UnlockedLevel = 0;
}
