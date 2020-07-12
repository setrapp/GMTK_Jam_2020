using System.Collections;
using UnityEngine;

public class AccelerantPin : MonoBehaviour
{
	private float timeRemaining = 0;
	private AccelerantTileData data;

	private Coroutine waiting = null;

	public bool Ticking => timeRemaining > 0;


	public void ResetTime(AccelerantTileData data)
	{
		this.data = data;
		timeRemaining = data.DetonateDelay;

		if (waiting == null)
		{
			waiting = StartCoroutine(triggerAfterDelay());
		}
	}

	private IEnumerator triggerAfterDelay()
	{
		while (timeRemaining > 0)
		{
			yield return null;
			timeRemaining -= Time.deltaTime;
		}

		timeRemaining = 0;
		waiting = null;
	}
}
