﻿using UnityEngine;

[CreateAssetMenu(fileName = "AccelerantTileType", menuName = "ScriptableObjects/TileTypes/AccelerantTileData")]
public class AccelerantTileData : TileData
{
	[SerializeField] private float detonateDelay = 0;
	public float DetonateDelay => detonateDelay;

	public override bool IsMatch(TileData other, Tile targetTile)
	{
		if (other != null && targetTile != null)
		{
			var pin = targetTile.GetComponent<AccelerantPin>();
			if (pin == null)
			{
				return false;
			}
			else
			{
				return pin.Ticking;
			}
		}

		return false;
	}

	protected override void burn(Tile target)
	{
		if (target != null)
		{
			var accelerantPin = target.GetComponent<AccelerantPin>();
			if (accelerantPin == null)
			{
				accelerantPin = target.gameObject.AddComponent<AccelerantPin>();
			}
		}
	}
}