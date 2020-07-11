using System.Collections;
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
	public Tile Tile => tile;

	public void CheckTile()
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

		tile.Data = data.tileData;
	}
}
