using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Levels
{
	public class LevelSelectItem : MonoBehaviour
	{
		public LevelSelect levelSelect = null;
		private Level level;
		private int levelIndex = 0;

		public void AttachLevel(Level level, int levelIndex)
		{
			this.level = level;
			this.levelIndex = levelIndex;
		}

		public void Event_LoadLevel()
		{
			levelSelect.levelManager.CurrentLevel = (uint)levelIndex;
			SceneManager.LoadScene(levelSelect.gameScene);
		}
	}
}