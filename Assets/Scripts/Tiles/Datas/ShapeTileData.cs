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

	public override bool IsMatch(Tile other)
	{
		if (other != null && other.Data == null)
		{

			var otherShape = other.Data as ShapeTileData;
			if (otherShape != null)
			{
				return otherShape.Shape == Shape;
			}
		}

		return false;
	}
}
