using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
	[SerializeField] private TileData data;
	[SerializeField] private Image image;
	public TileGridCell GridCell = null;


	[SerializeField] private Animator anim = null;
	private const string swapReady = "SwapReady";

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
				}
				else
				{
					image.sprite = null;
					image.gameObject.SetActive(false);
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

	public void Event_OnClick()
	{
		if (GridCell != null)
		{
			//if (GridCell.Grid.selectedCell == null)
			{
				GridCell.Grid.selectedCell = GridCell;
				if (anim != null)
				{
					anim.SetBool(swapReady, true);
				}
				else
				{
					Event_ShowSwapIndictors(true);
				}
			}
		}
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
			bool showUp = GridCell.Data.y < GridCell.Grid.GridHeight;
			bool showRight = GridCell.Data.x < GridCell.Grid.GridWidth;
			bool showDown = GridCell.Data.y > 0;
			bool showLeft = GridCell.Data.x > 0;

			GridCell.Grid.Swapper.transform.position = transform.position;
			GridCell.Grid.Swapper.ShowSides(showUp, showRight, showDown, showLeft);
		}
	}
}
