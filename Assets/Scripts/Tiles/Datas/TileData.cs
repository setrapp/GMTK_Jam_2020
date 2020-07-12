using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileData : ScriptableObject
{
	[SerializeField] private Sprite image = null;
	public Sprite Image => image;

	[SerializeField] private GameObject[] detonateFX;
	[SerializeField] private GameObject[] burnFX;

	public bool Swappable => true;

	public bool DestroyOnBurn = false;

	public abstract bool IsMatch(TileData other);
	public abstract bool IsBurnMatch(TileData other);

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
}
