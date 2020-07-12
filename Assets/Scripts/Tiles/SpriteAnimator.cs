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
	private int multiplierFramesPerSprite = 1;

	private int framesUntilSprite = 0;

	private void Awake()
	{
		image = GetComponent<Image>();
	}

	public void AttachSpriteSheet(Texture2D spriteSheet, int multiplierFramesPerSprite)
	{
		this.multiplierFramesPerSprite = multiplierFramesPerSprite;
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
			framesUntilSprite = Mathf.Abs(framesPerSprite * multiplierFramesPerSprite);
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

		spriteIndex = spriteIndex + (framesPerSprite * multiplierFramesPerSprite < 0
			? -1
			: 1);

		if (spriteIndex < 0)
		{
			spriteIndex = sprites.Length - 1;
		}
		else if (spriteIndex > sprites.Length - 1)
		{
			spriteIndex = 0;
		}

		image.sprite = sprites[spriteIndex];
	}
}