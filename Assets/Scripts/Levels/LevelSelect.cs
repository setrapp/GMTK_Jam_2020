using Levels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
	public LevelManager levelManager;
	[SerializeField] private LevelSelectItem listItem = null;
	[SerializeField] private Transform container = null;
	public string gameScene = "Game";

	void Awake()
	{
		for (int i = 0; i < levelManager.Levels.Length; i++)
		{
			var level = levelManager.Levels[i];
			var item = Instantiate(listItem, container).GetComponent<LevelSelectItem>();
			item.levelSelect = this;
			item.AttachLevel(level, i);
		}
	}
}
