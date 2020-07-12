using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Levels
{
	public class Score : MonoBehaviour
	{
		public static Score Instance = null;

		private int tilesInCombo = 0;
		private float timeToEndCombo = 2;
		private int recentCombo = 3;
		private int oldCombo = 1;
		private float lastPointScored = -1;
		private int comboLevel = 1;

		public int points = 0;

		public UnityEvent onLostCombo;
		public UnityEvent onNewCombo;

		public TextMeshProUGUI text = null;

		private void Awake()
		{
			Instance = this;
			text.text = "0";
		}

		void Update()
		{
			if (lastPointScored < 0 || (Time.time - lastPointScored) > timeToEndCombo)
			{
				recentCombo = 3;
				oldCombo = 1;

				if (comboLevel > 1)
				{
					onLostCombo.Invoke();
				}

				comboLevel = 1;
			}
		}

		public void ScoreTile()
		{
			lastPointScored = Time.time;
			points += comboLevel;
			tilesInCombo++;
			if (tilesInCombo > recentCombo + oldCombo)
			{
				comboLevel++;
				var temp = oldCombo;
				oldCombo = recentCombo;
				recentCombo = recentCombo + temp;
				onNewCombo.Invoke();
			}

			text.text = "" + points;
		}


	}
}