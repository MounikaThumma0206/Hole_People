using UnityEngine;

public class HoleGenerator : GridItemGenerator
{
	[SerializeField] bool isHole;
	[SerializeField] float Radius = 3.0f;


	internal override void Generate()
	{
		if (!isHole)
		{
			return;
		}
		Vector2Int minBounds = GetMinBounds();
		Vector2Int maxBounds = GetMaxBounds();
		int minX = minBounds.x;
		int minY = minBounds.y;
		int maxX = maxBounds.x;
		int maxY = maxBounds.y;

		Vector3 lowestPos = gridGenerator.GetWorldPosition(minX, minY, true);
		Vector3 highestPos = gridGenerator.GetWorldPosition(maxX, maxY, true);
		Vector3 center = (lowestPos + highestPos) / 2;
		transform.position = center;
		GameObject gridObject;
		for (int i = minX; i <= maxX; i++)
		{
			for (int j = minY; j <= maxY; j++)
			{
				if (gridGenerator.TryGetGridObject(i, j, out gridObject))
				{
					Destroy(gridObject);
				}
			}
		}
	}

#if UNITY_EDITOR

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		if (isHole)
		{
			Gizmos.DrawSphere(transform.position, Radius);
		}
	}
#endif
}