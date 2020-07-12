using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileData : ScriptableObject
{
	[SerializeField] private Sprite image = null;
	public Sprite Image => image;

	public bool Swappable => true;

	public abstract bool IsMatch(TileData other);
	public abstract bool IsBurnMatch(TileData other);

	// Destroy Tile and take neighbors with it.
	public void Detonate(Tile target)
	{
		if (target != null)
		{
			if (target.destroyState == Tile.DestroryState.None)
			{
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
				burn(target);
			}
		}
	}
	protected virtual void burn(Tile target)
	{
		RemoveFromGrid(target);
	}

	public void RemoveFromGrid(Tile target)
	{
		if (target.GridCell != null)
		{
			target.GridCell.Tile = null;
		}

		target.GridCell = null;
		Destroy(target.gameObject);
	}
}
