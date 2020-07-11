using System;
using System.Collections;
using System.Collections.Generic;
using Grid;
using TMPro;
using UnityEditorInternal;
using UnityEngine;

public class TileGridCell : MonoBehaviour
{
	[SerializeField] private TileGridCellData data = null;
	private TileGrid grid = null;
	[SerializeField] private Animator anim;

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
				tile.GridCell = this;
				if (anim != null)
				{
					StartCoroutine(RebindAnim());
				}
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


	public void HandleGridChange()
	{
		if (Tile != null)
		{
			Tile.HandleGridChange();
		}
	}

	public void GenerateTile(TileData tileData)
	{
		if (tile == null)
		{
			tile = Instantiate(Grid.TilePrefab, transform).GetComponent<Tile>();
		}

		tile.GridCell = this;
		tile.Data = tileData;
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
				if (IsValidMatch(neighbor))
				{
					return neighbor.MatchInLine(this);
				}
			}
		}

		return false;
	}

	public bool MatchInLine(TileGridCell other)
	{
		if (other == this || other == null)
		{
			return false;
		}

		int xDiff = Data.x - other.Data.x;
		int yDiff = Data.y - other.Data.y;

		var oppositeNeighbor = Grid.Cell(Data.x + xDiff, Data.y + yDiff);
		if (oppositeNeighbor != null)
		{
			if (oppositeNeighbor.IsValidMatch(other))
			{
				return true;
			}
		}

		var otherOppositeNeighbor = Grid.Cell(other.Data.x - xDiff, other.Data.y - yDiff);
		if (otherOppositeNeighbor)
		{
			if (otherOppositeNeighbor.IsValidMatch(this))
			{
				return true;
			}
		}

		return false;
	}

	private bool IsValidMatch(TileGridCell other)
	{
		if ((other != null && other != this)
		    && (other.Tile != null && other.TileReady)
		    && (Tile != null && TileReady))
		{
			return other.Tile.IsMatch(Tile);
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
			Grid.DetonateCells.Add(this);
		}

		var recurseNeighbors = new List<TileGridCell>();

		foreach (var neighbor in CardinalNeighbors())
		{
			bool burn = true;
			if (neighbor != null && neighbor.IsValidMatch(Grid.DetonateCells[0]))
			{
				if (MatchInLine(neighbor))
				{
					burn = false;
					if (!Grid.DetonateCells.Contains(neighbor))
					{
						Grid.DetonateCells.Add(neighbor);
						recurseNeighbors.Add(neighbor);
					}
				}

			}

			if (burn)
			{
				if (!Grid.BurnCells.Contains(neighbor))
				{
					Grid.BurnCells.Add(neighbor);
				}
			}
		}

		foreach (var neighbor in AllNeighbors())
		{
			if (neighbor != null && neighbor.Tile != null)
			{

			}
		}

		foreach (var neighbor in recurseNeighbors)
		{
			neighbor.BuildDestructionLists(false);
		}
	}

	private IEnumerator RebindAnim()//todo this does not seem to work
	{
		if (anim != null)
		{
			yield return null;
			anim.Rebind();
			anim.Update(Time.deltaTime);//TODO Is this required?
		}
	}

	public TileGridCell GetCellBelow()
	{
		return Grid.GetCellBelow(this);
	}
}
