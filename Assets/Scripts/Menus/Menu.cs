using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
	[SerializeField] bool quitOnEscape = false;

	private void Awake()
	{
		Application.targetFrameRate = 30;
	}

	private void Update()
	{
		if (quitOnEscape && Input.GetKeyDown(KeyCode.Escape))
		{
			Event_Quit();
		}
	}

	public void Event_GotoScene(string scene)
	{
		SceneCurtain.ChangeScene(scene);
	}

	public void Event_Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
