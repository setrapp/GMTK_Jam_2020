using Grid;
using UnityEngine;

[CreateAssetMenu(fileName = "GoalTileType", menuName = "ScriptableObjects/TileTypes/GoalTileData")]
public class GoalTileData : TileData
{
	public override void OnSpawn(Tile target, TileGrid grid)
	{
		grid.activeGoals.Add(target);
	}

	public override bool IsMatch(TileData other, Tile targetTile)
	{
		if (other != null && other is GoalTileData)
		{
			return true;
		}

		return false;
	}

	protected override void detonate(Tile target)
	{
		GameMenu.Instance.Win();
		base.detonate(target);
	}

	protected override void burn(Tile target)
	{
		if (DestroyOnBurn)
		{
			if (target.GridCell != null)
			{
				target.GridCell.Grid.activeGoals.Remove(target);
			}
		}
		base.burn(target);
	}
}
