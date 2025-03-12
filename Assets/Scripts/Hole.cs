using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Hole : GridItemGenerator
{
	public static UnityEvent<ColorEnum> OnAnyHoleClicked = new();
	//public CroudManager playerGenerator; // Reference to PlayerGridGenerator
	[SerializeField] private float jumpDetectionRadius;
	[SerializeField] private float holeRadius = 1;
	[SerializeField] AudioSource audioSource;
	[SerializeField] AudioClip noMovesClip;
	[SerializeField] private Animator _animator;
	[SerializeField] bool canClose;
	[SerializeField] GameObject closableHoleCanvas;
	[SerializeField] Image fill;
	public UnityEvent OnHoleClosed = new();
	private int _totalPeopleToBeAttracted = 0;
	private int _totalPeople;
	public float JumpDetectionRadius { get => jumpDetectionRadius; }
	public float HoleRadius => holeRadius;

#if UNITY_EDITOR
	[Header("Gizmos")]
	[SerializeField] float Radius = 3.0f;
#endif

	private void Awake()
	{
		if (canClose)
		{
			closableHoleCanvas.SetActive(true);
		}
		else
		{
			closableHoleCanvas.SetActive(false);
		}
	}
	public override void OnEnable()
	{
		base.OnEnable();
		GameManager.Instance.SusbscribeHole(this);
	}
	public override void OnDisable()
	{
		base.OnDisable();
		GameManager.Instance.UnSubscribeHole(this);
	}


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

#if UNITY_EDITOR
	private void OnValidate()
	{
		if (closableHoleCanvas != null && closableHoleCanvas.activeInHierarchy != canClose)
		{
			closableHoleCanvas.SetActive(canClose);
		}
	}

#endif



	private void CloseHole()
	{
		if (_animator != null)
		{
			_animator.SetTrigger("Close");
		}
		OnHoleClosed?.Invoke();
		obstacle.enabled = false;
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
	internal void CloseHoleAfterEating(int peopleCount)
	{
		if (canClose)
		{
			_totalPeopleToBeAttracted = peopleCount;
			_totalPeople = peopleCount;
			GridElement.OnGridElementJumped.AddListener(CheckForClosing);
		}
	}


	private void CheckForClosing(GridElement gridElement)
	{
		if (gridElement.PlayerColor != colorEnum)
		{
			return;
		}
		_totalPeopleToBeAttracted--;
		fill.fillAmount = 1f - (_totalPeopleToBeAttracted / (float)_totalPeople);
		if (_totalPeopleToBeAttracted <= 0)
		{
			CloseHole();
		}
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


