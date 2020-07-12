using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Levels
{
	public class LevelSelectItem : MonoBehaviour
	{
		public LevelSelect levelSelect = null;
		private Level level;
		private int levelIndex = 0;

		[SerializeField] private TextRenderer text = null;

		public void AttachLevel(Level level, int levelIndex)
		{
			this.level = level;
			this.levelIndex = levelIndex;

			var levelName = level.name;
			if (levelName.Contains('_'))
			{
				levelName = levelName.Substring(levelName.IndexOf('_') + 1);
			}
			text.SetText(levelName);
		}

		public void Event_LoadLevel()
		{
			levelSelect.levelManager.CurrentLevel = (uint)levelIndex;
		}
	}
}