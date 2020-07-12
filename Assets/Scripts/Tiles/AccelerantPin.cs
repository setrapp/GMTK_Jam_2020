using System;
using System.Collections;
using UnityEngine;

public class AccelerantPin : MonoBehaviour
{
	private float timeRemaining = 0;
	private AccelerantTileData data;

	private Coroutine waiting = null;

	public bool Ticking = true;

	public void SetData(AccelerantTileData data)
	{
		this.data = data;
	}

	private void OnEnable()
	{
		timeRemaining = -1;
		Ticking = true;
	}

	public void ResetTime()
	{
		timeRemaining = data.DetonateAfter;

		if (waiting == null)
		{
			waiting = StartCoroutine(attemptTriggersOvertime());
		}
	}

	private IEnumerator attemptTriggersOvertime()
	{
		Ticking = true;

		var tile = GetComponent<Tile>();
		if (tile != null && tile.GridCell != null)
		{
			tile.GridCell.CheckForTriplet();
		}

		while (timeRemaining > 0)
		{
			yield return null;
			timeRemaining -= Time.deltaTime;
		}

		if (tile != null && tile.GridCell != null)
		{
			tile.GridCell.CheckForTriplet();
			tile.GridCell.Grid.StartCoroutine(tile.GridCell.Grid.DetonateAndBurn(tile.GridCell));
		}


		Ticking = false;

		timeRemaining = -1;
		waiting = null;
	}
}
