using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileData : ScriptableObject
{
	[SerializeField] private Sprite image = null;
	public Sprite Image => image;

	public abstract bool IsMatch(Tile other);

	public virtual void Detonate(Tile target)
	{
		if (target != null)
		{
			Destroy(target.gameObject);
		}
	}
}
