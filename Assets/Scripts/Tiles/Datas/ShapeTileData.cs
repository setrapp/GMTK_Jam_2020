﻿using UnityEngine;

[CreateAssetMenu(fileName = "ShapeTileType", menuName = "ScriptableObjects/TileTypes/ShapeTileData")]
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
