using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI fpsCounter;
	int updateRate = 10;
	private void Update()
	{
		if (Time.frameCount % updateRate == 0)
		{
			fpsCounter.text = Mathf.CeilToInt(1 / Time.deltaTime).ToString();
		}
	}
}
