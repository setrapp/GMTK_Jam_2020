using UnityEngine;

[CreateAssetMenu(fileName = "ShapeTileType", menuName = "ScriptableObjects/TileTypes/ShapeTileData")]
public class ShapeTileData : TileData
{
	public enum TileShape
	{
		Shape0 = 0,
		Shape1 = 1,
		Shape2 = 2
	}

	private TileShape shape = TileShape.Shape0;

	public TileShape Shape
	{
		get { return shape; }
		set { shape = value; }
	}

	public override bool IsMatch(TileData other)
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

	public override bool IsBurnMatch(TileData other)
	{
		return IsMatch(other);
	}
}
