using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Represents a hole in the grid system that can attract entities and optionally close after a set number of entities enter.
/// </summary>
public class Hole : GridItemGenerator
{
	/// <summary>
	/// Event triggered when any hole is clicked, passing the associated color.
	/// </summary>
	public static UnityEvent<ColorEnum> OnAnyHoleClicked = new();

	[SerializeField] private float jumpDetectionRadius; // Radius to detect jumping entities.
	[SerializeField] private float holeRadius = 1; // Radius of the hole.
	[SerializeField] private AudioSource audioSource; // Audio source for playing sounds.
	[SerializeField] private AudioClip noMovesClip; // Sound to play when there are no available moves.
	[SerializeField] private Animator _animator; // Animator component for hole animations.
	[SerializeField] private bool canClose; // Determines if the hole can be closed.
	[SerializeField] private GameObject closableHoleCanvas; // UI element indicating a closable hole.
	[SerializeField] private Image fill; // UI fill bar for closing progress.
	[SerializeField] private ParticleSystem carrotFx;
	[SerializeField] private ParticleSystem carrotEatFX;

	/// <summary>
	/// Event triggered when the hole is closed.
	/// </summary>
	public UnityEvent OnHoleClosed = new();

	private int _totalPeopleToBeAttracted = 0; // Tracks the number of people needed to close the hole.
	private int _totalPeople; // Total number of people expected to enter before closure.

	/// <summary>
	/// Gets the jump detection radius.
	/// </summary>
	public float JumpDetectionRadius => jumpDetectionRadius;

	/// <summary>
	/// Gets the hole radius.
	/// </summary>
	public float HoleRadius => holeRadius;

	/// <summary>
	/// Gets the color associated with the hole.
	/// </summary>
	public ColorEnum ColorEnum => colorEnum;

#if UNITY_EDITOR
	[Header("Gizmos")]
	[SerializeField] private float Radius = 3.0f; // Editor visualization radius.
#endif

	/// <summary>
	/// Initializes the hole's state on awake.
	/// </summary>
	private void Awake()
	{
		closableHoleCanvas.SetActive(canClose);
	}

	/// <summary>
	/// Subscribes the hole to the GameManager and relevant events when enabled.
	/// </summary>
	public override void OnEnable()
	{
		base.OnEnable();
		_totalPeopleToBeAttracted = 0;
		OnAnyHoleClicked.AddListener(PlayCarrot);
		GameManager.Instance.SusbscribeHole(this);
		GridElement.OnGridElementJumped.AddListener(PlayCarrotFx);

		if (canClose)
		{
			GridElement.OnGridElementJumped.AddListener(CheckForClosing);
		}
	}

	void PlayCarrot(ColorEnum @enum)
	{
		if (@enum == colorEnum)
		{
			carrotFx.Play();
		}
	}
	/// <summary>
	/// Unsubscribes the hole from the GameManager when disabled.
	/// </summary>
	public override void OnDisable()
	{
		base.OnDisable();
		GameManager.Instance.UnSubscribeHole(this);
	}

	/// <summary>
	/// Generates the hole by determining its position in the grid and removing conflicting grid objects.
	/// </summary>
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
	/// <summary>
	/// Ensures UI elements match the hole's state in the editor.
	/// </summary>
	private void OnValidate()
	{
		if (closableHoleCanvas != null && closableHoleCanvas.activeInHierarchy != canClose)
		{
			closableHoleCanvas.SetActive(canClose);
		}
	}
#endif

	internal void PlayCarrotFx(GridElement gridElement)
	{
		if (gridElement.PlayerColor == colorEnum)
		{
			//carrotFx.Play();
			carrotEatFX.Play();
		}
	}
	/// <summary>
	/// Closes the hole, triggers animations, and invokes closure events.
	/// </summary>
	private void CloseHole()
	{
		if (_animator != null)
		{
			_animator.SetTrigger("Close");
		}
		OnHoleClosed?.Invoke();
		obstacle.enabled = false;
	}

	/// <summary>
	/// Handles player interactions when the hole is clicked.
	/// </summary>
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
			GameManager.Instance.UseMove();
			OnAnyHoleClicked.Invoke(colorEnum);
			transform.DOPunchScale(transform.localScale * .1f, .1f, 1, 1);
		}
		else
		{
			Debug.LogWarning("GameManager reference is missing in Hole.");
		}
	}

	/// <summary>
	/// Plays a sound when the player has no available moves.
	/// </summary>
	internal void PlayNoMoves()
	{
		audioSource.PlayOneShot(noMovesClip);
		CroudHaptics.PlayHeavyHaptics();
	}

	/// <summary>
	/// Sets the number of people expected to enter before the hole closes.
	/// </summary>
	/// <param name="peopleCount">The total number of people required to close the hole.</param>
	internal void CloseHoleAfterEating(int peopleCount)
	{
		if (canClose)
		{
			_totalPeople = peopleCount;
		}
	}

	/// <summary>
	/// Checks if the hole should be closed based on the number of people entering.
	/// </summary>
	/// <param name="gridElement">The grid element that has jumped into the hole.</param>
	private void CheckForClosing(GridElement gridElement)
	{
		if (gridElement.PlayerColor != colorEnum)
		{
			return;
		}

		_totalPeopleToBeAttracted++;
		fill.fillAmount = (float)(_totalPeopleToBeAttracted * 10 / ((float)_totalPeople * 10));

		if (_totalPeopleToBeAttracted >= _totalPeople)
		{
			CloseHole();
		}
	}

#if UNITY_EDITOR
	/// <summary>
	/// Draws gizmos for debugging the hole in the Unity Editor.
	/// </summary>
	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawSphere(transform.position, Radius);
	}

	/// <summary>
	/// Draws wireframe spheres to visualize detection radii in the Unity Editor.
	/// </summary>
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, JumpDetectionRadius);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, holeRadius);
	}
#endif
}
