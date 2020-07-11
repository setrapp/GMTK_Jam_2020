using System.Collections;
using System.Collections.Generic;
using Grid;
using UnityEngine;

public class TileGridCell : MonoBehaviour
{
	[SerializeField] private TileGridCellData data = null;
	private TileGrid grid = null;

	public TileGrid Grid
	{
		get { return grid; }
		set { grid = value; }
	}

	public TileGridCellData Data
	{
		get { return data; }
		set { data = value; }
	}

	private Tile tile = null;

	public Tile Tile
	{
		get { return tile; }
		set
		{
			if (tile != value)
			{
				tile = value;
				data.tileData = tile.Data;
				tile.GridCell = this;
			}
		}
	}

	public bool TileReady => Tile != null
		? Tile.LockedIn
		: false;

	public IEnumerable<TileGridCell> CardinalNeighbors()
	{
		if (data.y < Grid.GridHeight)
		{
			// Up
			yield return Grid.Cell(data.x, data.y + 1);
		}
		if (data.x < Grid.GridWidth)
		{
			// Right
			yield return Grid.Cell(data.x + 1, data.y);
		}
		if (data.y > 0)
		{
			// Down
			yield return Grid.Cell(data.x, data.y - 1);
		}
		if (data.x > 0)
		{
			// Left
			yield return Grid.Cell(data.x - 1, data.y);
		}
	}

	public IEnumerable<TileGridCell> AllNeighbors()
	{
		bool hasUp = data.y < Grid.GridHeight;
		bool hasRight = data.x < Grid.GridWidth;
		bool hasDown = data.y > 0;
		bool hasLeft = data.x > 0;

		if (hasUp)
		{
			// Up
			yield return Grid.Cell(data.x, data.y + 1);
			if (hasRight)
			{
				// Up-Right
				yield return Grid.Cell(data.x + 1, data.y + 1);
			}
		}
		if (hasRight)
		{
			// Right
			yield return Grid.Cell(data.x + 1, data.y);
			if (hasDown)
			{
				// Right-Down
				yield return Grid.Cell(data.x + 1, data.y - 1);
			}
		}
		if (hasDown)
		{
			// Down
			yield return Grid.Cell(data.x, data.y - 1);
			if (hasLeft)
			{
				// Down-Left
				yield return Grid.Cell(data.x - 1, data.y - 1);
			}
		}
		if (hasLeft)
		{
			// Left
			yield return Grid.Cell(data.x - 1, data.y);
			if (hasUp)
			{
				// Left-Up
				yield return Grid.Cell(data.x - 1, data.y + 1);
			}
		}
	}

	public bool IsNeighbor(TileGridCell other, bool requireCardinal)
	{
		if (other == null || other == this)
		{
			return false;
		}

		if (requireCardinal)
		{
			foreach (var neighbor in CardinalNeighbors())
			{
				if (neighbor == other)
				{
					return true;
				}
			}
		}
		else
		{
			foreach (var neighbor in AllNeighbors())
			{
				if (neighbor == other)
				{
					return true;
				}
			}
		}

		return false;
	}

	public void ValidateTile()
	{
		if (data != null)
		{
			if (data.tileData != null)
			{
				if (tile == null || tile.Data != data.tileData)
				{
					GenerateTile();
				}
			}
			else
			{
				tile.Data = null;
			}
		}
	}

	public void GenerateTile()
	{
		if (tile == null)
		{
			tile = Instantiate(Grid.TilePrefab, transform).GetComponent<Tile>();
		}

		tile.GridCell = this;
		tile.Data = data.tileData;
	}

	public bool CanBeginSwap()
	{
		if (!TileReady)
		{
			return false;
		}

		return Grid != null
			? Grid.CanBeginSwap(this)
			: false;
	}

	public void MakeSwapTarget()
	{
		if (Grid != null)
		{
			Grid.ChooseSwapTarget(this);
		}
	}

	public void UnmakeSwapTarget()
	{
		if (Tile != null)
		{
			Tile.Event_Unselect();
		}
	}

	public void SwapTile()
	{
		Grid.SwapTileWithSelected(this);
	}
}
