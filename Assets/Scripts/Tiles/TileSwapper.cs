using UnityEngine;

public class TileSwapper : MonoBehaviour
{
	[SerializeField] private GameObject swapUp = null;
	[SerializeField] private GameObject swapRight = null;
	[SerializeField] private GameObject swapDown = null;
	[SerializeField] private GameObject swapLeft = null;

	[SerializeField] private float longFactor = 1;
	[SerializeField] private float shortFactor = 0.1f;

	public void SetSideSize(float tileSize)
	{
		((RectTransform) transform).sizeDelta = new Vector2(tileSize, tileSize);
	}

	public void HideSides()
	{
		ShowSides(false, false, false, false);
	}

	public void ShowSides(bool showUp, bool showRight, bool showDown, bool showLeft)
	{
		swapUp.SetActive(showUp);
		swapRight.SetActive(showRight);
		swapDown.SetActive(showDown);
		swapLeft.SetActive(showLeft);
	}
}
