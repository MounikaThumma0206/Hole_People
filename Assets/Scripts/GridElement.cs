using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using DT_Util;

/// <summary>
/// Represents an element in the grid that can move, jump, and interact with holes.
/// </summary>
public class GridElement : MonoBehaviour
{
	/// <summary>
	/// Event triggered when a GridElement jumps.
	/// </summary>
	public static UnityEvent<GridElement> OnGridElementJumped = new();

	public int lineIndex; // Index position in a line formation.
	public CroudManager playerGenerator; // Reference to the crowd manager.
	public Color gizmoColor = Color.yellow; // Color used for editor gizmos.
	public int Row; // The row position in the grid.
	public int Column; // The column position in the grid.
	public bool BlockedPath; // Determines if this grid element blocks movement.
	public bool IsOccupied; // Checks if this grid element is occupied.
	public bool IsEmpty; // Checks if this grid element is empty.
	public ColorEnum PlayerColor; // The color of the player on this grid element.
	public GameObject Player; // Reference to the player GameObject.
	public SkinnedMeshRenderer playerRenderer; // Renderer for changing player appearance.
	public Animator animator; // Animator for movement/jump animations.
	public Vector3 PlayerInitialPos; // The initial position of the player.
	public Vector3 PlayerInitialScale; // The initial scale of the player.
	public bool StartedRunning; // Tracks if the entity has started running.
	[SerializeField] private ParticleSystem dirtSmokeFx;
	private bool isRefilling; // Flag for refill status.
	public Hole Hole; // Reference to the hole the entity is associated with.
	public NavMeshAgent agent; // AI agent for navigation.
	private Vector3? targetPosition; // Target position for movement.

	// Variables for jump handling
	[SerializeField] private Vector3 jumpStartPosition, jumpMidPosition, jumpEndPosition;
	[SerializeField] private float jumpSpeed;
	[SerializeField] private float jumpInterpTime = 0;
	private bool canJump = false;

	/// <summary>
	/// Gets the radius of the associated hole.
	/// </summary>
	private float HoleRadius => Hole.HoleRadius;

	/// <summary>
	/// Gets the jump detection radius of the hole.
	/// </summary>
	private float JumpDetectionRadius => Hole.JumpDetectionRadius;

	/// <summary>
	/// Gets whether the grid element is filled.
	/// </summary>
	public bool IsFilled { get; private set; } = false;

	/// <summary>
	/// Initializes the grid element.
	/// </summary>
	void Start()
	{
		// Offset idle animation clip for animation variety
		if (animator != null)
		{
			AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
			animator.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
		}
	}

	/// <summary>
	/// Initializes jump parameters.
	/// </summary>
	public void StartJumping(Vector3 jumpStartPos, Vector3 jumpMidPoint, Vector3 jumpEndPos)
	{
		jumpStartPosition = jumpStartPos;
		jumpMidPosition = jumpMidPoint;
		jumpEndPosition = jumpEndPos;
	}

	/// <summary>
	/// Checks if the entity is within stopping distance of the hole.
	/// </summary>
	/// <returns>True if within stopping distance, otherwise false.</returns>
	private bool IsWithinStoppingDistance()
	{
		return Hole != null && (Hole.transform.position - agent.transform.position).sqrMagnitude < JumpDetectionRadius * JumpDetectionRadius;
	}

	/// <summary>
	/// Coroutine executed when the destination is reached, triggering jump behavior.
	/// </summary>
	private IEnumerator OnReachedDestination()
	{
		StartedRunning = false;

		if (animator != null)
		{
			animator.SetTrigger("Jump");
		}
		agent.enabled = false;

		if (transform.childCount > 0)
		{
			GameObject player = transform.GetChild(0).gameObject;

			if (Hole != null)
			{
				agent.enabled = false;
				canJump = true;
				dirtSmokeFx.Stop();
				StartJumping(
					agent.transform.position,
					Hole.transform.position + Vector3.up * 4 + VectorExt.GetRandomDirectionalVector() * HoleRadius,
					Hole.transform.position + Vector3.down * 2 + VectorExt.GetRandomDirectionalVector() * HoleRadius);

				yield return new WaitForSeconds(1f);

				player.SetActive(false);
				transform.gameObject.SetActive(false);
			}
			else
			{
				Debug.LogWarning("Hole reference is not set!");
			}
		}
		else
		{
			Debug.LogWarning("No children found under this transform!");
		}
	}

	/// <summary>
	/// Determines if the entity has reached the hole.
	/// </summary>
	/// <returns>True if within range of the hole, otherwise false.</returns>
	public bool HasReachedHole()
	{
		return transform != null && Hole != null && Vector3.Distance(transform.position, Hole.transform.position) <= 0.5f;
	}

	/// <summary>
	/// Marks the grid element as filled.
	/// </summary>
	public void MarkAsFilled()
	{
		IsFilled = true;
	}

	/// <summary>
	/// Resets the filled status of the grid element.
	/// </summary>
	public void ResetFillStatus()
	{
		IsFilled = false;
	}

	/// <summary>
	/// Changes the material of the player.
	/// </summary>
	public void ChangePlayerMaterial(Material material)
	{
		playerRenderer.sharedMaterial = material;
	}

	/// <summary>
	/// Moves the entity to a specified position.
	/// </summary>
	public void MoveToPosition(Vector3 pipeMouthPosition)
	{
		targetPosition = transform.position;
		agent.transform.position = pipeMouthPosition;

		agent.enabled = false;
		agent.transform.forward = (targetPosition.Value - pipeMouthPosition).normalized;
		isRefilling = true;
		animator.SetTrigger("Run");
	}

	/// <summary>
	/// Updates the crowd movement logic.
	/// </summary>
	public void OnCrowdUpdate()
	{
		if (!StartedRunning && !targetPosition.HasValue && !canJump)
		{
			return;
		}

		if (StartedRunning && IsWithinStoppingDistance())
		{
			StartCoroutine(OnReachedDestination());
		}

		if (canJump)
		{
			jumpInterpTime += Time.deltaTime * jumpSpeed;
			var agentNewPos = VectorExt.CubicBezier(jumpStartPosition, jumpMidPosition, jumpEndPosition, jumpInterpTime);
			agent.transform.position = agentNewPos;
			if (jumpInterpTime > 1)
			{
				jumpInterpTime = 1;
				canJump = false;
				agent.gameObject.SetActive(false);
				CrowdAudioManager.PlayJumpSound();
				OnGridElementJumped?.Invoke(this);
			}
		}

		if (targetPosition.HasValue)
		{
			Vector3 distanceVector = targetPosition.Value - agent.transform.position;
			if (distanceVector.sqrMagnitude < 0.01f)
			{
				targetPosition = null;
				isRefilling = false;
				// Offset idle animation clip for animation variety
				if (animator != null)
				{
					AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
					animator.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
				}
				agent.transform.position = transform.position;
				agent.transform.forward = Vector3.back;
				animator.SetTrigger("Idle");
				return;
			}

			// Adjust movement speed based on distance
			float magnitude = distanceVector.magnitude;
			float agentMoveSpeed = 7;
			float speed = magnitude > 1 ? agentMoveSpeed : agentMoveSpeed * magnitude;
			agent.transform.Translate(Time.deltaTime * speed * distanceVector.normalized, Space.World);
		}
	}

#if UNITY_EDITOR
	/// <summary>
	/// Draws debugging gizmos in the Unity Editor.
	/// </summary>
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawSphere(agent.transform.position, .2f);
		Gizmos.DrawSphere(jumpStartPosition, .2f);
		Gizmos.DrawSphere(jumpMidPosition, .2f);
		Gizmos.DrawSphere(jumpEndPosition, .2f);
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(VectorExt.CubicBezier(jumpStartPosition, jumpMidPosition, jumpEndPosition, jumpInterpTime), .2f);
	}
#endif
}
