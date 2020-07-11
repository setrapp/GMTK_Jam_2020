using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
	[SerializeField] private TileData data;
	[SerializeField] private Image image;

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
}
