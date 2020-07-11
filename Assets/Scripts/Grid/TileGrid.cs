using System;
using System.Collections.Generic;
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

		public Tile TilePrefab => tilePrefab;

		private TileGridCell[,] cells;

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

			var length = SetupData.Length;
			var width = SetupData.Width;

			var rectTransform = transform as RectTransform;
			container.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y * ((float)width / length));
			float cellSize = container.sizeDelta.x / length;
			float startX = (-rectTransform.sizeDelta.x / 2) + (cellSize / 2);
			float startY = (-rectTransform.sizeDelta.y / 2) + (cellSize / 2);

			// Fill grid with new cells.
			cells = new TileGridCell[length, width];
			for (uint i = 0; i < length; i++)
			{
				for (uint j = 0; j < width; j++)
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
				if (cellData.x < length && cellData.y < width)
				{
					cells[cellData.x, cellData.y].Data = cellData;
				}
			}

			// Fill the undefined cells with random tiles
			for (uint i = 0; i < length; i++)
			{
				for (uint j = 0; j < width; j++)
				{
					var cell = cells[i, j];
					if (cell.Data == null)
					{
						cell.Data = new TileGridCellData()
						{
							tileData = null,
							x = i,
							y = j
						};
					}

					if (cell.Data.tileData == null)
					{
						cell.Data.tileData = setupData.GetRandomTileData();
					}
				}
			}

			// Actually build the tiles for all this data!
			foreach (var cell in cells)
			{
				cell.GenerateTile();
			}
		}

	}
}