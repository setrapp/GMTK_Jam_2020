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

	protected override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void Win()
	{
		//TODO Do something better
		onWin.Invoke();
	}

	public void Event_NextLevel()
	{
		levelManager.CurrentLevel++;
		SceneManager.LoadScene(nextLevelScene);
	}

	public override void Event_Quit()
	{
		Event_GotoScene("MainMenu");
	}
}
