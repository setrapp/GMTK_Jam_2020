using System;
using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteAnimator : MonoBehaviour
{
	private Image image = null;
	private Sprite[] sprites = null;
	private int spriteIndex = -1;

	[SerializeField] private int framesPerSprite = 1;
	private int framesUntilSprite = 0;

	private void Awake()
	{
		image = GetComponent<Image>();
	}

	public void AttachSpriteSheet(Texture2D spriteSheet)
	{
		sprites = null;
		if (spriteSheet == null)
		{
			image.sprite = null;
			return;
		}

		sprites = Resources.LoadAll<Sprite>(spriteSheet.name);
		NextSprite();
	}

	private void Update()
	{
		if (framesUntilSprite <= 0)
		{
			framesUntilSprite = framesPerSprite;
			NextSprite();
		}
		else
		{
			framesUntilSprite--;
		}
	}

	private void NextSprite()
	{
		if (sprites == null || sprites.Length < 1)
		{
			image.sprite = null;
			return;
		}

		spriteIndex = (spriteIndex + 1) % sprites.Length;
		image.sprite = sprites[spriteIndex];
	}
}