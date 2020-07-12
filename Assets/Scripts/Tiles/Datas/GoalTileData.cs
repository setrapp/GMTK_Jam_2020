using Grid;
using UnityEngine;

[CreateAssetMenu(fileName = "GoalTileData", menuName = "ScriptableObjects/TileDatas/GoalTileData")]
public class GoalTileData : TileData
{
	[SerializeField] private bool isSwappable = true;


	public override void OnSpawn(Tile target, TileGrid grid)
	{
		Swappable = isSwappable;
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

	public virtual void FallIntoPlace(Tile target)
	{
		if (target != null && target.GridCell != null)
		{
			target.GridCell.CheckForTriplet();
		}
	}

}
