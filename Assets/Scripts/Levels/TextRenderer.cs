using System;
using TMPro;
using UnityEngine;

public class TextRenderer : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI text = null;

	public void SetText(string text)
	{
		this.text.text = text;
	}
}
