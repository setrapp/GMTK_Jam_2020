using System;
using System.Collections;
using System.Collections.Generic;
using Grid;
using TMPro;
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
		if (data.y < Grid.GridHeight - 1)
		{
			// Up
			yield return Grid.Cell(data.x, data.y + 1);
		}

		if (data.x < Grid.GridWidth - 1)
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

	public IEnumerable<TileGridCell> NonCardinalNeighbors()
	{
		bool hasUp = data.y < Grid.GridHeight;
		bool hasRight = data.x < Grid.GridWidth;
		bool hasDown = data.y > 0;
		bool hasLeft = data.x > 0;

		if (hasUp && hasRight)
		{
			// Up-Right
			yield return Grid.Cell(data.x + 1, data.y + 1);
		}

		if (hasRight && hasDown)
		{
			// Right-Down
			yield return Grid.Cell(data.x + 1, data.y - 1);
		}

		if (hasDown && hasLeft)
		{
			// Down-Left
			yield return Grid.Cell(data.x - 1, data.y - 1);
		}

		if (hasLeft && hasUp)
		{
			// Left-Up
			yield return Grid.Cell(data.x - 1, data.y + 1);
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

	public void CheckForTriplet()
	{
		if (InTriplet())
		{
			BuildDestructionLists(true);
		}
	}

	public bool InTriplet()
	{
		if (Tile == null)
		{
			return false;
		}

		int matches = 0;
		foreach (var neighbor in CardinalNeighbors())
		{
			if (neighbor != null && neighbor.Tile != null)
			{
				if (Tile.IsMatch(neighbor.Tile))
				{
					matches++;
					if (neighbor.OppositeNeighborMatches(this))
					{
						matches++;
					}
				}
			}
		}

		foreach (var neighbor in NonCardinalNeighbors())
		{
			if (neighbor != null && neighbor.Tile != null)
			{
				if (Tile.IsMatch(neighbor.Tile))
				{
					matches++;
				}
			}
		}

		return matches > 1;
	}

	public bool OppositeNeighborMatches(TileGridCell other)
	{
		if (other == null || other.Data == null || Data == null || other.Tile == null)
		{
			return false;
		}

		int xDiff = Data.x - other.Data.x;
		int yDiff = Data.y - other.Data.y;

		int oppositeX = Data.x + xDiff;
		int oppositeY = Data.y + yDiff;

		var oppositeNeighbor = Grid.Cell(oppositeX, oppositeY);
		if (oppositeNeighbor != null && oppositeNeighbor.Tile != null)
		{
			return other.Tile.IsMatch(oppositeNeighbor.Tile);
		}

		return false;
	}

	public void BuildDestructionLists(bool firstCell)
	{
		// TODO This method thing is very expensive... maybe we can optimize it.

		if (Grid == null)
		{
			return;
		}


		if (firstCell)
		{
			clearBuildLists();
			Grid.DetonateCells.Add(this);
		}

		var recurseNeighbors = new List<TileGridCell>();

		foreach (var neighbor in CardinalNeighbors())
		{
			if (neighbor != null && neighbor.Tile != null && neighbor.Tile.IsMatch(Tile))
			{
				if (!Grid.DetonateCells.Contains(neighbor))
				{
					Grid.DetonateCells.Add(neighbor);
					recurseNeighbors.Add(neighbor);
				}
			}
		}

		foreach (var neighbor in AllNeighbors())
		{
			if (neighbor != null && neighbor.Tile != null && neighbor.Tile.IsMatch(Tile))
			{
				if (!Grid.BurnCells.Contains(neighbor))
				{
					// TODO handle cells that we we'll want to detonate later... gotta figure out a clean way to do that.
					Grid.BurnCells.Add(neighbor);
				}
			}
		}
	}

	private void clearBuildLists()
	{
		if (Grid != null)
		{
			if (Grid.DetonateCells == null)
			{
				Grid.DetonateCells = new List<TileGridCell>();
			}
			if (Grid.BurnCells == null)
			{
				Grid.BurnCells = new List<TileGridCell>();
			}
			if (Grid.DetonateLaterCells == null)
			{
				Grid.DetonateLaterCells = new List<TileGridCell>();
			}
		}

		Grid.DetonateCells.Clear();
		Grid.BurnCells.Clear();
		Grid.DetonateLaterCells.Clear();

	}
}
