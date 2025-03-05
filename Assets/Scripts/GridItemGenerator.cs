using UnityEngine;

public class GridItemGenerator : MonoBehaviour
{
	[SerializeField] protected GridGenerator gridGenerator;
	[SerializeField] protected ColorEnum colorEnum;
	[SerializeField] protected DebugGizmosColor gizmoColor;
	[SerializeField] protected Vector3 boxSize;
	public Vector3 BoxSize
	{
		get => boxSize;
		set => boxSize = value;
	}

	public virtual void OnEnable()
	{
		if (gridGenerator == null) transform.root.GetComponent<GridGenerator>();
		gridGenerator.Subscribe(this);
	}
	public virtual void OnDisable()
	{
		gridGenerator.UnSubscribe(this);
	}
	public Vector2Int GetMinBounds()
	{
		gridGenerator.GetGridPosition(transform.position - MultiplyVectors(BoxSize / 2f, transform.lossyScale), out int minX, out int minY);

		return new(minX, minY);
	}
	public Vector2Int GetMaxBounds()
	{
		gridGenerator.GetGridPosition(transform.position + MultiplyVectors(BoxSize / 2f, transform.lossyScale), out int maxX, out int maxY);

		return new(maxX, maxY);
	}
	protected Vector3 MultiplyVectors(Vector3 a, Vector3 b)
	{
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	internal virtual void Generate()
	{

	}
#if UNITY_EDITOR
	protected virtual void OnDrawGizmos()
	{
		if(gizmoColor == null)
		{
			return;
		}
		Gizmos.color = gizmoColor.GetGizmoColor(colorEnum);

		if (gridGenerator == null)
		{
			return;
		}

		Vector2Int minBounds = GetMinBounds();
		Vector2Int maxBounds = GetMaxBounds();
		int minX = minBounds.x;
		int minY = minBounds.y;
		int maxX = maxBounds.x;
		int maxY = maxBounds.y;
		for (int x = minX; x <= maxX; x++)
		{
			for (int y = minY; y <= maxY; y++)
			{
				Gizmos.DrawSphere(gridGenerator.GetWorldPosition(x, y, true), 0.1f);
			}
		}
	}
#endif
}
