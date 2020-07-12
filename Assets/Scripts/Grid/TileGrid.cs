using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		[SerializeField] private bool ignoreBurnInEditor = false;
		public TileSwapper Swapper => swapper;

		private TileGridCell selectedCell = null;

		public Tile TilePrefab => tilePrefab;

		private TileGridCell[,] cells;
		public int GridWidth => cells.GetLength(0);
		public int GridHeight => cells.GetLength(1);

		private Dictionary<TileGridCell, List<DetonateCellData>> DetonateCells = null;

		[SerializeField] private int detonateOrderDelay = 1;

		[SerializeField] private float populateTopDelay = 1;
		private float timeUntilPopulateTop = 0;

		private void Start()
		{
			GenerateGrid(levelManager.Levels[levelManager.CurrentLevel]);
			DetonateCells = new Dictionary<TileGridCell, List<DetonateCellData>>();
			timeUntilPopulateTop = populateTopDelay;
		}

		private void Update()
		{
			timeUntilPopulateTop -= Time.deltaTime;
			if (timeUntilPopulateTop <= 0)
			{
				PopulateTopCells();
				timeUntilPopulateTop = populateTopDelay;
			}
		}

		public void GenerateGrid(Level data)
		{
			StartCoroutine(generateGrid(data));
		}

		private IEnumerator generateGrid(Level data)
		{
			var rectTransform = transform as RectTransform;

			if (data != null)
			{
				SetupData = data;
			}

			var width = SetupData.Width;
			var height = SetupData.Height;

			container.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.width * ((float) height / width));
			float cellSize = container.sizeDelta.x / width;
			float startX = (-rectTransform.rect.width / 2) + (cellSize / 2);
			float startY = (-rectTransform.rect.height / 2) + (cellSize / 2);

			// Fill grid with new cells.
			cells = new TileGridCell[width, height];
			var predefinedTiles = new TileData[width, height];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					cells[i, j] = Instantiate(cellPrefab, container).GetComponent<TileGridCell>();
					cells[i, j].Grid = this;
					((RectTransform) cells[i, j].transform).sizeDelta = new Vector2(cellSize, cellSize);
					((RectTransform) cells[i, j].transform).anchoredPosition =
						new Vector2(startX + (cellSize * i), startY + (cellSize * j));

					predefinedTiles[i, j] = null;
				}
			}

			// Populate defined cells.
			foreach (var cellData in SetupData.GetCells())
			{
				if (cellData.x < width && cellData.y < height)
				{
					cells[cellData.x, cellData.y].Data = new TileGridCellData()
					{
						x = cellData.x,
						y = cellData.y
					};

					predefinedTiles[cellData.x, cellData.y] = cellData.tileData;
				}
			}

			// Fill the undefined cells with random tiles
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					var cell = cells[i, j];
					if (cell.Data == null || predefinedTiles[i, j] == null)
					{
						cell.Data = new TileGridCellData()
						{
							x = i,
							y = j
						};
						predefinedTiles[i, j] = setupData.GetRandomTileData();
					}
				}
			}

			// Actually build the tiles for all this data!
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					var cell = cells[i, j];
					cell.GenerateTile(predefinedTiles[i, j]);
				}
			}

			swapper.SetSideSize(cellSize);
			swapper.HideSides();

			yield break;
		}

		public TileGridCell Cell(int x, int y)
		{
			if ((x < 0 || x >= GridWidth)
			    || (y < 0 || y >= GridHeight))
			{
				return null;
			}

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

			selectedCell.CheckForTriplet();
			other.CheckForTriplet();

			StartCoroutine(DetonateAndBurn(selectedCell));
			StartCoroutine(DetonateAndBurn(other));

			ChooseSwapTarget(null);
			return true;
		}

		public TileGridCell GetCellBelow(TileGridCell cell)
		{
			if (cell == null || cell.Data == null)
			{
				return null;
			}

			//TODO Handle rotation;
			return Cell(cell.Data.x, cell.Data.y - 1);
		}

		public bool PrepareToDetonate(TileGridCell toDetonate, int order, TileGridCell startingCell)
		{
			if (startingCell == null)
			{
				return false;
			}

			if (!DetonateCells.ContainsKey(startingCell))
			{
				DetonateCells.Add(startingCell, new List<DetonateCellData>());
				DetonateCells[startingCell].Add(new DetonateCellData()
				{
					cell = startingCell,
					order = 0
				});

				return true;
			}

			bool alreadyTracked = false;
			bool alteredList = false;
			foreach (var cellData in DetonateCells[startingCell])
			{
				if (cellData.cell == toDetonate)
				{
					alreadyTracked = true;
					if (order < cellData.order)
					{
						cellData.order = order;
						alteredList = true;
					}

					break;
				}
			}

			if (!alreadyTracked)
			{
				alteredList = true;
				DetonateCells[startingCell].Add(new DetonateCellData()
				{
					cell = toDetonate,
					order = order
				});
			}

			return alteredList;
		}

		public IEnumerator DetonateAndBurn(TileGridCell starter)
		{
			if (starter != null && DetonateCells.ContainsKey(starter))
			{
				List<DetonateCellData> detonateData = DetonateCells[starter];

				int order = 0;
				bool done = false;

				while (!done)
				{
					var toDetonate = new List<TileGridCell>();

					for (int i = 0; i < detonateData.Count; i++)
					{
						// TODO Remove detonatedCells from list (make sure this doesn't break list checks when adding news (like weird loops)

						var cell = detonateData[i];
						if (cell != null && cell.cell != null && cell.cell.Tile != null && cell.order == order)
						{
							toDetonate.Add(cell.cell);
							detonateData.RemoveAt(i);
							i--;
						}
					}

					if (toDetonate.Count > 0)
					{
						foreach (var cell in toDetonate)
						{
							if (cell != null && cell.Tile != null)
							{
								cell.Tile.Detonate();

								foreach (var neighbor in cell.CardinalNeighbors())
								{
									var burn = neighbor.TileReady;
#if UNITY_EDITOR
									burn = burn && !ignoreBurnInEditor;
#endif
									if (burn && !toDetonate.Contains(neighbor))
									{
										if (neighbor.Tile != null)
										{
											neighbor.Tile.Burn();
										}
									}
								}
							}
						}

						yield return null;

						foreach (var cell in cells)
						{
							if (cell != null)
							{
								cell.HandleGridChange();
							}
						}

						for (int i = 0; i < detonateOrderDelay; i++)
						{
							yield return null;
						}

						order++;
					}
					else
					{
						done = true;
					}
				}

				DetonateCells[starter].Clear();
				DetonateCells.Remove(starter);
			}
		}

		public void PopulateTopCells()
		{
			// TODO Handle rotation
			int y = GridHeight - 1;
			int cellsOnTop = GridWidth;

			for (int i = 0; i < cellsOnTop; i++)
			{
				if (cells[i, y].Tile == null)
				{
					cells[i, y].GenerateTile(setupData.GetRandomTileData());
				}
			}
		}
	}

	public class DetonateCellData
	{
		public TileGridCell cell;
		public int order = 0;
	}
}