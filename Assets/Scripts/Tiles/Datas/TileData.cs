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
	public virtual void Detonate(Tile target)
	{
		if (target != null)
		{
			Destroy(target.gameObject);
		}
	}

	// Destroy Tile but leave neighbors alone.
	public virtual void Burn(Tile target)
	{
		if (target != null)
		{
			Destroy(target.gameObject);
		}
	}
}
