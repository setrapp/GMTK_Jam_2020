using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
	[SerializeField] bool quitOnEscape = false;

	protected virtual void Awake()
	{
		Application.targetFrameRate = 60;
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
