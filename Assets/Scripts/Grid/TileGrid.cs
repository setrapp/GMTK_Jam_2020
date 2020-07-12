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
		[SerializeField] public RectTransform orphanTileContainer = null;
		[SerializeField] private TileSwapper swapper = null;

		public List<Tile> activeGoals = null;
		private int spawnsUntilGoalAllowed = 0;

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
				StartCoroutine(PopulateTopCells());
				timeUntilPopulateTop = populateTopDelay;
			}
		}

		public void GenerateGrid(Level data)
		{
			StartCoroutine(generateGrid(data));
		}

		private IEnumerator generateGrid(Level data)
		{
			activeGoals = new List<Tile>();

			var rectTransform = transform as RectTransform;

			if (data != null)
			{
				SetupData = data;
			}

			var width = SetupData.Width;
			var height = SetupData.Height;

			//container.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.width * ((float) height / width));
			//orphanTileContainer.sizeDelta = container.sizeDelta;

			float cellSize = ((RectTransform) cellPrefab.transform).rect.width;

			float wouldBeCellSize = rectTransform.rect.width / width;
			float scaleFactor = wouldBeCellSize / cellSize;
			transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
			container.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
			orphanTileContainer.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

			float startX = ((-rectTransform.rect.width / 2) / scaleFactor) + (cellSize / 2);
			float startY = ((-rectTransform.rect.height / 2) / scaleFactor) + (cellSize / 2);

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

			//swapper.SetSideSize(cellSize);
			swapper.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
			swapper.HideSides();

			transform.localScale = new Vector3(1, 1, 1);

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

		public bool CanSwapTileWithSelected(TileGridCell other)
		{
			if (other == null || other == selectedCell || !other.TileReady || !selectedCell.TileReady)
			{
				return false;
			}

			return true;
		}

		public void PrepareSwapTileWithSelected(TileGridCell other)
		{
			var up = "MoveUp";
			var right = "MoveRight";
			var down = "MoveDown";
			var left = "MoveLeft";

			if (other.Data.x > selectedCell.Data.x)
			{
				other.AnimateMove(left);
				selectedCell.AnimateMove(right);
			}
			else if (other.Data.x < selectedCell.Data.x)
			{
				other.AnimateMove(right);
				selectedCell.AnimateMove(left);
			}
			else if (other.Data.y > selectedCell.Data.y)
			{
				other.AnimateMove(down);
				selectedCell.AnimateMove(up);
			}
			else if (other.Data.y < selectedCell.Data.y)
			{
				other.AnimateMove(up);
				selectedCell.AnimateMove(down);
			}
		}

		public void SwapTileWithSelected(TileGridCell other)
		{
			if (other == selectedCell || selectedCell == null)
			{
				return;
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
			Swapper.HideSides();
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
					tile = startingCell.Tile,
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
					tile = toDetonate.Tile,
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
				DetonateCells.Remove(starter);

				int order = 0;
				bool done = false;

				while (!done)
				{
					var toDetonate = new List<TileGridCell>();

					for (int i = 0; i < detonateData.Count; i++)
					{
						// TODO Remove detonatedCells from list (make sure this doesn't break list checks when adding news (like weird loops)

						var cell = detonateData[i];
						if (cell != null && cell.cell != null && cell.cell.Tile != null && cell.order == order && cell.tile == cell.cell.Tile)
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

				detonateData.Clear();
			}
		}

		public IEnumerator PopulateTopCells()
		{
			// TODO Handle rotation
			int y = GridHeight - 1;
			int cellsOnTop = GridWidth;

			for (int i = 0; i < cellsOnTop; i++)
			{
				if (cells[i, y].Tile == null)
				{
					bool forceGoal = false;
					if (spawnsUntilGoalAllowed <= 0)
					{
						if (activeGoals.Count < setupData.maxGoalCount)
						{
							var nearestGoal = -1;
							foreach (var goal in activeGoals)
							{
								if (goal.GridCell == null)
								{
									// If the goal does not have a cell, assume it is falling and don't allow another goal to spawn.
									nearestGoal = 0;
								}
								else
								{
									var goalDist = Mathf.Abs(goal.GridCell.Data.x - i) + Mathf.Abs(goal.GridCell.Data.y - y);
									if (nearestGoal < 0 || goalDist < nearestGoal)
									{
										nearestGoal = goalDist;
									}
								}
							}

						if ((nearestGoal < 0 || nearestGoal >= (int)(setupData.minGoalDistanceForSize * GridWidth))
						    && Random.Range(0f, 1f) < setupData.chanceToSpawnGoal)
							{
								forceGoal = true;
								spawnsUntilGoalAllowed = setupData.spawnsBetweenGoals;
							}
						}
					}
					else
					{
						spawnsUntilGoalAllowed--;
					}

					Tile newTile = null;
					if (!forceGoal)
					{
						newTile = cells[i, y].GenerateTile(setupData.GetRandomTileData());
					}
					else
					{
						newTile = cells[i, y].GenerateTile(setupData.goalTileData);
					}

					if (newTile != null)
					{
						newTile.isNewTile = true;
					}
				}
			}

			yield return null;

			for (int i = 0; i < cellsOnTop; i++)
			{
				cells[i, y].HandleGridChange();
			}
		}
	}

	public class DetonateCellData
	{
		public TileGridCell cell;
		public Tile tile;
		public int order = 0;
	}
}