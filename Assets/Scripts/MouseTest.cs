using UnityEngine;

public class MouseTest : MonoBehaviour
{
	Vector3 mousePos;
	private void Update()
	{
		if ((Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)))
		{
			mousePos = hit.point;
		}
	}
	private void OnDrawGizmos()
	{
		Gizmos.DrawSphere(mousePos, 0.5f);
	}
}