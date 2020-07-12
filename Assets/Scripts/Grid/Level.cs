using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Levels/Level")]
public class Level : ScriptableObject
{
	[SerializeField] private int width = 5;
	public int Width => width;
	[SerializeField] private int height = 5;
	public int Height => height;

	public LevelTileGridCellData[] cells;

	[SerializeField] RandomTileData[] allowedRandomTiles = null;

	[SerializeField] public GoalTileData goalTileData = null;
	[SerializeField] public float chanceToSpawnGoal = 1;
	[SerializeField] public int maxGoalCount = 3;
	[SerializeField] public int spawnsBetweenGoals = 5;
	[SerializeField] public float minGoalDistanceForSize = 0.5f;
	[SerializeField] public string helpText = null;

	public IEnumerable<LevelTileGridCellData> GetCells()
	{
		foreach (var cell in cells)
		{
			yield return cell;
		}
	}

	public TileData GetRandomTileData()
	{
		if (allowedRandomTiles == null || allowedRandomTiles.Length < 1)
		{
			return null;
		}

		float total = 0;
		foreach (var tile in allowedRandomTiles)
		{
			total += tile.weight;
		}

		float choice = Random.Range(0, total);

		foreach (var tile in allowedRandomTiles)
		{
			if (tile.weight > choice)
			{
				return tile.tileData;
			}
			else
			{
				choice -= tile.weight;
			}
		}

		return null;
	}
}

[System.Serializable]
public class TileGridCellData
{
	public int x;
	public int y;
}

[System.Serializable]
public class LevelTileGridCellData : TileGridCellData
{
	public TileData tileData;
}

[System.Serializable]
public class RandomTileData
{
	public TileData tileData = null;
	public float weight = 1;
}
