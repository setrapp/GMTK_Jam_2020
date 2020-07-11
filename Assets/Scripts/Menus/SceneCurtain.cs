using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCurtain : MonoBehaviour
{
	private static SceneCurtain instance = null;
	private static SceneCurtain Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<SceneCurtain>();
			}

			return instance;
		}
	}

	[SerializeField] private Animator anim = null;
	[SerializeField] private string exitSceneParam = "Exit";
	[SerializeField] private string enterSceneParam = "Enter";

	private string gotoScene = null;
	private bool prepareEnterAfterExit = false;

	public static void ChangeScene(string scene)
	{
		if (Instance == null)
		{
			SceneManager.LoadScene(scene);
			return;
		}

		Instance.gotoScene = scene;
		instance.prepareEnterAfterExit = true;
		instance.PrepareExitScene();
	}

	private void PrepareExitScene()
	{
		if (anim != null)
		{
			anim.SetTrigger(exitSceneParam);
		}
		else
		{
			Event_ExitReady();
		}
	}

	public void PrepareEnterScene()
	{
		if (anim != null)
		{
			anim.SetTrigger(enterSceneParam);
		}
		else
		{
			Event_EnterReady();
		}
	}

	public void Event_ExitReady()
	{
		SceneManager.LoadScene(gotoScene);
		if (prepareEnterAfterExit)
		{
			PrepareEnterScene();
		}
	}

	public void Event_EnterReady()
	{

	}
}
