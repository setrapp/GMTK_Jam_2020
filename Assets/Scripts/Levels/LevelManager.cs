using System;
using UnityEngine;

[CreateAssetMenu (fileName = "LevelManager", menuName = "ScriptableObjects/Levels/LevelManager")]
public class LevelManager : ScriptableObject
{
	[SerializeField] private Level[] levels = null;
	public Level[] Levels => levels;

	[NonSerialized] public uint CurrentLevel = 0;
	[NonSerialized] public uint UnlockedLevel = 0;
}
