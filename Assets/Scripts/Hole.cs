using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Hole : GridItemGenerator
{
	public static UnityEvent<ColorEnum> OnAnyHoleClicked = new();
	public ColorEnum colorEnum;
	//public CroudManager playerGenerator; // Reference to PlayerGridGenerator
	[SerializeField] private float jumpDetectionRadius;
	[SerializeField] private float holeRadius = 1;
	[SerializeField] AudioSource audioSource;
	[SerializeField] AudioClip noMovesClip;
	public float JumpDetectionRadius { get => jumpDetectionRadius; }
	public float HoleRadius => holeRadius;
#if UNITY_EDITOR
	[Header("Gizmos")]
	[SerializeField] float Radius = 3.0f;
#endif

	internal override void Generate()
	{
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



	private void OnMouseDown()
	{
		if (GameManager.Instance != null)
		{
			if (!GameManager.Instance.IsGameOn())
			{
#if UNITY_EDITOR
				Debug.Log("Game is not on");
#endif
				return;

			}
			GameManager.Instance.IsPlayerMovable(colorEnum, this);

			//// Use the reference to move the player to the hole if playerGridGenerator is assigned
			//if (playerGenerator != null)
			//{
			//	//Debug.Log("Hole clicked");
			//	playerGenerator.movePlayerToHole(this);
			//}
			////else
			////{
			////	Debug.LogWarning("PlayerGridGenerator reference is missing in Hole.");
			////}
			GameManager.Instance.UseMove();
			OnAnyHoleClicked.Invoke(colorEnum);
			transform.DOPunchScale(transform.localScale * .1f, .1f, 1, 1);
		}
		else
		{
			Debug.LogWarning("GameManager reference is missing in Hole.");
		}

	}

	internal void PlayNoMoves()
	{
		audioSource.PlayOneShot(noMovesClip);
	}

	private void OnTriggerEnter(Collider other)
	{
		//Jump
		Debug.Log(other.gameObject.name);
		/* if (other.gameObject.CompareTag("Stickman"))
			 other.gameObject.SetActive(false);*/
	}
#if UNITY_EDITOR

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();

		Gizmos.DrawSphere(transform.position, Radius);

	}
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, JumpDetectionRadius);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, holeRadius);
	}
#endif
}


