using UnityEngine;

[CreateAssetMenu(fileName = "RotateTileData", menuName = "ScriptableObjects/TileDatas/RotateTileData")]
public class RotateTileData : TileData
{
	[SerializeField] int rotateDirection = 1;

	public override bool IsMatch(TileData other, Tile targetTile)
	{
		if (other is RotateTileData)
		{
			return true;
		}
		return false;
	}

	protected override void burn(Tile target)
	{
		if (target != null && target.GridCell != null)
		{
			target.GridCell.Grid.Rotate(90 * rotateDirection);
		}
		base.burn(target);
	}
}
