using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameMenu : Menu
{
	private static GameMenu instance = null;
	public static GameMenu Instance => instance;

	public UnityEvent onWin = null;

	[SerializeField] private LevelManager levelManager;

	[SerializeField] private string nextLevelScene = null;

	private bool won = false;

	protected override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void Win()
	{
		if (!won)
		{
			if (levelManager.UnlockedLevel == levelManager.CurrentLevel)
			{
				levelManager.CurrentLevel++;
				levelManager.UnlockCurrentLevel();
			}

			//TODO Do something better
			onWin.Invoke();
			won = true;
		}
	}

	public void Event_NextLevel()
	{
		SceneManager.LoadScene(nextLevelScene);
	}

	public override void Event_Quit()
	{
		Event_GotoScene("MainMenu");
	}
}
