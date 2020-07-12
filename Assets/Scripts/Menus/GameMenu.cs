using System;
using UnityEngine;
using UnityEngine.Events;

public class GameMenu : Menu
{
	private static GameMenu instance = null;
	public static GameMenu Instance => instance;

	public UnityEvent onWin = null;

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
}
