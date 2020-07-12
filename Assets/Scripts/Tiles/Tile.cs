using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	public TileGridCell GridCell = null;
	[SerializeField] private Button button = null;


	[SerializeField] private Animator anim = null;
	private const string swapReady = "SwapReady";

	public bool LockedIn = true;

	public DestroryState destroyState = DestroryState.None;

	private TileGridCell fallToCell = null;
	private Coroutine fallRoutine = null;

	[SerializeField] private float maxFallSpeed = 1;

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
					image.sprite = data.Image;
					image.gameObject.SetActive(true);
					button.interactable = data.Swappable;
				}
				else
				{
					image.sprite = null;
					image.gameObject.SetActive(false);
					button.interactable = false;
				}
			}
		}
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
		transform.SetParent(GridCell.transform);
		transform.localPosition = Vector3.zero;
	}

	public void FallToGridCell()
	{
		if (GridCell == null)
		{
			return;
		}

		// TODO Fall like gravity
		transform.SetParent(GridCell.transform);
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
		return Data.IsMatch(other.Data) || other.Data.IsMatch(Data);
	}

	public bool IsBurnMatch(Tile other)
	{
		if (other == null || other.Data == null || Data == null)
		{
			return false;
		}

		// Check if either tile thinks they are a match, because they might have different criteria.
		// Matching only requires one tile succeed.
		return Data.IsBurnMatch(other.Data) || other.Data.IsBurnMatch(Data);
	}

	public void Detonate()
	{
		data.Detonate(this);
	}

	public void Burn()
	{
		data.Burn(this);
	}

	public void HandleGridChange()
	{
		checkForFalling();
	}

	private void checkForFalling()
	{
		// TODO This is gonna get weird with rotation

		var cellBelow = GridCell.GetCellBelow();
		while (cellBelow != null && cellBelow.Tile == null && cellBelow.awaitingFallingTile == null)
		{
			fallToCell = cellBelow;
			cellBelow = cellBelow.GetCellBelow();
		}

		if (fallToCell != null && fallRoutine == null)
		{
			fallRoutine = StartCoroutine(fall());
		}
	}

	private IEnumerator fall()
	{
		if (GridCell != null)
		{
			removeFromCell();
			fallToCell.awaitingFallingTile= this;
		}

		bool stillFalling = true;
		while (stillFalling)
		{
			// TODO Handle rotation.
			var localPos = transform.localPosition;
			localPos.y -= maxFallSpeed;

			if (localPos.y < fallToCell.transform.localPosition.y)
			{
				localPos.y = fallToCell.transform.localPosition.y;
				stillFalling = false;
			}

			transform.localPosition = localPos;
			yield return null;
		}

		fallToCell.awaitingFallingTile = null;
		fallToCell.Tile = this;

		fallRoutine = null;
		fallToCell = null;
		LockedIn = true;

		HandleGridChange();

		yield return null;
		if (GridCell != null)
		{
			GridCell.CheckForTriplet();
			GridCell.Grid.StartCoroutine(GridCell.Grid.DetonateAndBurn(GridCell));
		}
	}

	public void removeFromCell()
	{
		LockedIn = false;
		transform.parent = GridCell.Grid.orphanTileContainer;

		if (GridCell != null)
		{
			GridCell.Tile = null;
		}

		GridCell = null;
	}
}
