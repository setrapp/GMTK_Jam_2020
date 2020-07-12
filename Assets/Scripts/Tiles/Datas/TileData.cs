using System;
using System.Collections;
using System.Collections.Generic;
using Grid;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class TileData : ScriptableObject
{
	[SerializeField] private Texture2D image = null;
	public Texture2D Image => image;

	public int framesPerSprite = 1;

	[SerializeField] private GameObject[] detonateFX;
	[SerializeField] private GameObject[] burnFX;

	[NonSerialized] public bool Swappable = true;

	public bool DestroyOnBurn = false;

	public virtual void OnSpawn(Tile target, TileGrid grid) { }

	public abstract bool IsMatch(TileData other, Tile targetTile);

	// Destroy Tile and take neighbors with it.
	public void Detonate(Tile target)
	{
		if (target != null)
		{
			if (target.destroyState == Tile.DestroryState.None)
			{
				if (target.GridCell != null && detonateFX.Length > 0)
				{
					target.GridCell.ShowDetonate(detonateFX[Random.Range(0, detonateFX.Length)]);
				}

				detonate(target);
			}
		}
	}

	protected virtual void detonate(Tile target)
	{
		RemoveFromGrid(target);
	}

	// Destroy Tile but leave neighbors alone.
	public void Burn(Tile target)
	{
		if (target != null)
		{
			if (target.destroyState == Tile.DestroryState.None)
			{
				if (target.GridCell != null && burnFX.Length > 0)
				{
					target.GridCell.ShowBurn(burnFX[Random.Range(0, burnFX.Length)]);
				}

				burn(target);
			}
		}
	}
	protected virtual void burn(Tile target)
	{
		if (DestroyOnBurn)
		{
			RemoveFromGrid(target);
		}
	}

	public void RemoveFromGrid(Tile target)
	{
		if (target != null)
		{
			target.removeFromCell();
			Destroy(target.gameObject);
		}
	}

	public virtual void FallIntoPlace(Tile Target) { }
}
