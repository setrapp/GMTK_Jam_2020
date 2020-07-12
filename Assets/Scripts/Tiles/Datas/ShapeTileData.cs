using UnityEngine;

[CreateAssetMenu(fileName = "ShapeTileData", menuName = "ScriptableObjects/TileDatas/ShapeTileData")]
public class ShapeTileData : TileData
{
	public enum TileShape
	{
		Circle = 0,
		Square = 1,
		Triangle = 2
	}

	[SerializeField] private TileShape shape = TileShape.Circle;

	public TileShape Shape
	{
		get { return shape; }
		set { shape = value; }
	}

	public override bool IsMatch(TileData other, Tile targetTile)
	{
		if (other != null)
		{
			var otherShape = other as ShapeTileData;
			if (otherShape != null)
			{
				return otherShape.Shape == Shape;
			}
		}

		return false;
	}
}
