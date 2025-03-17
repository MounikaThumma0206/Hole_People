using UnityEngine;

/// <summary>
/// Controls the camera's orthographic size, allowing dynamic resizing at runtime.
/// Implements a singleton pattern to ensure only one instance exists.
/// </summary>
public class CameraSizeChanger : MonoBehaviour
{
	/// <summary>
	/// Singleton instance of the CameraSizeChanger.
	/// </summary>
	public static CameraSizeChanger instance;

	/// <summary>
	/// Reference to the Camera component attached to the GameObject.
	/// </summary>
	private Camera cam;

	/// <summary>
	/// Ensures a single instance of CameraSizeChanger exists and initializes the camera reference.
	/// </summary>
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}

		cam = GetComponent<Camera>();
	}

	/// <summary>
	/// Changes the orthographic size of the camera.
	/// </summary>
	/// <param name="newSize">The new orthographic size for the camera.</param>
	public void ChangeSize(float newSize)
	{
		cam.orthographicSize = newSize;
	}
}
