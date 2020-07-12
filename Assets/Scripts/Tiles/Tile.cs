using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class Tile : MonoBehaviour
{
	public enum DestroryState
	{
		None = 0,
		Detonate,
		Burn
	}

	[SerializeField] private TileData data = null;
	[SerializeField] private Image image = null;
	[SerializeField] private SpriteAnimator spriteAnimator;
	public TileGridCell GridCell = null;
	[SerializeField] private Button button = null;


	[SerializeField] private Animator anim = null;
	private const string swapReady = "SwapReady";

	public bool LockedIn = true;

	public DestroryState destroyState = DestroryState.None;

	private TileGridCell fallToCell = null;
	private Coroutine fallRoutine = null;

	[SerializeField] private float waitFramesBeforeFall = 0;
	[SerializeField] private float maxFallSpeed = 1;
	[SerializeField] private bool rematchAfterFall = true;

	private List<TileGridCell> fallThroughTiles = null;

	public TileData Data
	{
		get { return data; }
		set
		{
			if (data != value)
			{
				data = value;
				if (data != null)
				{
					spriteAnimator.AttachSpriteSheet(data.Image, data.framesPerSprite);
					spriteAnimator.gameObject.SetActive(true);
					button.interactable = data.Swappable;
				}
				else
				{
					spriteAnimator.AttachSpriteSheet(null, 1);
					spriteAnimator.gameObject.SetActive(true);
					button.interactable = false;
				}
			}
		}
	}

	private void Update()
	{
		// TODO I would rather not have all the tiles updating all the time, but this falling bug is very annoying.
		HandleGridChange();
	}

	public void Reset()
	{
		if (Data != null)
		{
			Data = null;
		}
	}

	public void Event_Select()
	{
		if (Data != null && Data.Swappable && LockedIn)
		{
			if (GridCell != null)
			{
				if (GridCell.CanBeginSwap())
				{
					GridCell.MakeSwapTarget();
					if (anim != null)
					{
						anim.SetBool(swapReady, true);
					}
					else
					{
						Event_ShowSwapIndictors(true);
					}
				}
				else
				{
					GridCell.SwapTile();
				}
			}
		}
	}

	public void Event_Unselect()
	{
		if (anim != null)
		{
			anim.SetBool(swapReady, false);
		}

		Event_ShowSwapIndictors(false);
	}

	public void Event_ShowSwapIndictors(bool show)
	{
		if (GridCell == null)
		{
			return;
		}

		if (!show)
		{
			GridCell.Grid.Swapper.HideSides();
		}
		else
		{
			bool showUp = GridCell.Data.y < GridCell.Grid.GridHeight - 1;
			bool showRight = GridCell.Data.x < GridCell.Grid.GridWidth - 1;
			bool showDown = GridCell.Data.y > 0;
			bool showLeft = GridCell.Data.x > 0;

			GridCell.Grid.Swapper.transform.position = transform.position;
			GridCell.Grid.Swapper.ShowSides(showUp, showRight, showDown, showLeft);
		}
	}

	public void MoveToGridCell()
	{
		if (GridCell == null)
		{
			return;
		}

		// TODO Do this over time
		parentToCell(GridCell);
		transform.localPosition = Vector3.zero;
	}

	public bool IsMatch(Tile other)
	{
		if (other == null || other.Data == null || Data == null)
		{
			return false;
		}

		// Check if either tile thinks they are a match, because they might have different criteria.
		// Matching only requires one tile succeed.
		return Data.IsMatch(other.Data, this) || other.Data.IsMatch(Data, other);
	}

	public void Detonate()
	{
		data.Detonate(this);

		destroyState = DestroryState.Detonate;
	}

	public void Burn()
	{
		data.Burn(this);

		if (data.DestroyOnBurn)
		{
			destroyState = DestroryState.Burn;
		}
	}

	public void HandleGridChange()
	{
		if (fallToCell == null)
		{
			checkForFalling();
		}
		data.HandleBoardChanged(this);
	}

	private void checkForFalling()
	{
		// TODO This is gonna get weird with rotation
		if (GridCell == null)
		{
			return;
		}

		var cellBelow = GridCell.GetCellBelow();
		if (fallToCell == null)
		{
			fallThroughTiles = new List<TileGridCell>();
		}

		while (cellBelow != null && cellBelow.Tile == null && cellBelow.awaitingFallingTile == null)
		{
			fallThroughTiles.Add(cellBelow);
			fallToCell = cellBelow;
			cellBelow = cellBelow.GetCellBelow();
		}

		if (fallToCell != null && fallRoutine == null)
		{
			fallRoutine = StartCoroutine(fall());
		}

		if (fallToCell == null)
		{
			LockedIn = true;
		}
	}

	private IEnumerator fall()
	{
		for (int i = 0; i < waitFramesBeforeFall; i++)
		{
			yield return null;
		}

		if (GridCell != null)
		{
			removeFromCell();
			fallToCell.awaitingFallingTile= this;
		}

		bool stillFalling = true;
		while (stillFalling)
		{
			if (fallToCell.Grid.Rotating)
			{
				float bestDist = -1;
				TileGridCell bestCell = null;

				while (bestCell == null)
				{
					foreach (var cell in fallThroughTiles)
					{
						var sqrDist = (transform.position - cell.transform.position).sqrMagnitude;
						if (cell.Tile == null && cell.awaitingFallingTile && (bestDist < 0 || sqrDist < bestDist))
						{
							bestDist = sqrDist;
							bestCell = cell;
						}
					}

					if (bestCell == null)
					{
						yield return null;
					}
				}

				fallToCell.awaitingFallingTile = null;
				bestCell.Tile = this;
				parentToCell(bestCell, true);
				transform.localPosition = Vector3.zero;

				while (fallToCell.Grid.Rotating)
				{
					yield return null;
				}

				fallRoutine = null;
				fallToCell = null;

				yield break;
			}

			// TODO Handle rotation.
			var localPos = transform.localPosition;

			//localPos.y -= maxFallSpeed;
			localPos -= transform.InverseTransformDirection(Vector3.up) * maxFallSpeed;

			if (transform.position.y < fallToCell.transform.position.y)
			{
				localPos = fallToCell.transform.localPosition;
				stillFalling = false;
			}

			transform.localPosition = localPos;
			yield return null;
		}

		bool nowhereToFall = false;
		if ((fallToCell.awaitingFallingTile != null && fallToCell.awaitingFallingTile != this)
		    || (fallToCell.Tile && fallToCell.Tile != this))
		{
			nowhereToFall = true;
		}

		if (!nowhereToFall)
		{
			fallToCell.awaitingFallingTile = null;
			fallToCell.Tile = this;
			parentToCell(fallToCell, true);

			fallRoutine = null;
			fallToCell = null;
			LockedIn = true;

			HandleGridChange();

			data.FallIntoPlace(this);

			if (rematchAfterFall)
			{
				yield return null;
				if (GridCell != null)
				{
					GridCell.CheckForTriplet();
					GridCell.Grid.StartCoroutine(GridCell.Grid.DetonateAndBurn(GridCell));
				}
			}
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void removeFromCell()
	{
		LockedIn = false;
		transform.SetParent(GridCell.Grid.orphanTileContainer);

		if (GridCell != null)
		{
			GridCell.Tile = null;
		}

		GridCell = null;
	}

	private void parentToCell(TileGridCell cell, bool destroyIfConflict = false)
	{
		/*if (destroyIfConflict && cell.tileContainer.transform.childCount > 0)
		{
			Destroy(cell.transform.GetChild(0).gameObject);
			cell.Tile = this;
		}*/

		transform.SetParent(GridCell.tileContainer.transform);
	}
}
