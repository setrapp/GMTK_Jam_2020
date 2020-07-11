using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Grid
{
	public class TileGrid : MonoBehaviour
	{
		[SerializeField] private Level setupData = null;
		private Level SetupData
		{
			get { return setupData; }
			set { setupData = value; }
		}

		[SerializeField] private LevelManager levelManager = null;
		[SerializeField] private Tile tilePrefab = null;
		[SerializeField] private TileGridCell cellPrefab = null;
		[SerializeField] private RectTransform container = null;
		[SerializeField] private TileSwapper swapper = null;
		public TileSwapper Swapper => swapper;

		private TileGridCell selectedCell = null;

		public Tile TilePrefab => tilePrefab;

		private TileGridCell[,] cells;
		public int GridWidth => cells.GetLength(0);
		public int GridHeight => cells.GetLength(1);


		private void Start()
		{
			GenerateGrid(levelManager.Levels[levelManager.CurrentLevel]);
		}

		public void GenerateGrid(Level data)
		{
			if (data != null)
			{
				SetupData = data;
			}

			var width = SetupData.Width;
			var height = SetupData.Height;

			var rectTransform = transform as RectTransform;
			container.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y * ((float)height / width));
			float cellSize = container.sizeDelta.x / width;
			float startX = (-rectTransform.sizeDelta.x / 2) + (cellSize / 2);
			float startY = (-rectTransform.sizeDelta.y / 2) + (cellSize / 2);

			// Fill grid with new cells.
			cells = new TileGridCell[width, height];
			for (uint i = 0; i < width; i++)
			{
				for (uint j = 0; j < height; j++)
				{
					cells[i, j] = Instantiate(cellPrefab, container).GetComponent<TileGridCell>();
					cells[i, j].Grid = this;
					((RectTransform)cells[i, j].transform).sizeDelta = new Vector2(cellSize, cellSize);
					((RectTransform) cells[i, j].transform).anchoredPosition =
						new Vector2(startX + (cellSize * i), startY + (cellSize * j));
				}
			}

			// Populate defined cells.
			foreach (var cellData in SetupData.GetCells())
			{
				if (cellData.x < width && cellData.y < height)
				{
					cells[cellData.x, cellData.y].Data = cellData;
				}
			}

			// Fill the undefined cells with random tiles
			for (uint i = 0; i < width; i++)
			{
				for (uint j = 0; j < height; j++)
				{
					var cell = cells[i, j];
					if (cell.Data == null || cell.Data.tileData == null)
					{
						cell.Data = new TileGridCellData()
						{
							tileData = setupData.GetRandomTileData(),
							x = i,
							y = j
						};
					}
				}
			}

			// Actually build the tiles for all this data!
			foreach (var cell in cells)
			{
				cell.GenerateTile();
			}

			swapper.SetSideSize(cellSize);
			swapper.HideSides();
		}

		public TileGridCell Cell(uint x, uint y)
		{
			return cells[x, y];
		}

		public bool CanBeginSwap(TileGridCell cell)
		{
			if (cell != null)
			{
				if (selectedCell == null)
				{
					return true;
				}

				// If the cell is a cardinal neighbor, we'll want to swap with it, not let it take our swap.
				if (selectedCell.IsNeighbor(cell, true))
				{
					return false;
				}

				// The cell is not a relevant neighbor, so it can take the swap.
				return true;
			}

			return false;
		}

		public void ChooseSwapTarget(TileGridCell cell)
		{
			if (selectedCell != null)
			{
				selectedCell.UnmakeSwapTarget();
			}

			selectedCell = cell;
		}

		public bool SwapTileWithSelected(TileGridCell other)
		{
			if (other == null || other == selectedCell || !other.TileReady || !selectedCell.TileReady)
			{
				return false;
			}

			var tempTile = other.Tile;
			other.Tile = selectedCell.Tile;
			selectedCell.Tile = tempTile;

			selectedCell.Tile.MoveToGridCell();
			other.Tile.MoveToGridCell();

			ChooseSwapTarget(null);

			return true;
		}
	}
}