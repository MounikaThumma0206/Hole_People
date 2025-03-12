using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
using UnityEngine.Events;
using DT_Util;

public class GridElement : MonoBehaviour
{
	public static UnityEvent<GridElement> OnGridElementJumped = new();


	public ColorEnum GridColor;
	public int lineIndex;
	public CroudManager playerGenerator;
	public Color gizmoColor = Color.yellow;
	public int Row;
	public int Column;
	public bool BlockedPath;
	public bool IsOccupied;
	public bool IsEmpty;
	public ColorEnum PlayerColor;
	public GameObject Player;
	public SkinnedMeshRenderer playerRenderer;
	public Rigidbody rb;
	public Animator animator;
	public bool IsFilled { get; private set; } = false;
	public Vector3 PlayerInitialPos;
	public Vector3 PlayerInitialScale;
	public bool StartedRunning;
	private bool isRefilling;
	public Hole Hole;
	public NavMeshAgent agent;
	private float HoleRadius => Hole.HoleRadius;
	private float JumpDetectionRadius => Hole.JumpDetectionRadius;
	Vector3? targetPosition;

	//For jump
	[SerializeField] Vector3 jumpStartPosition, jumpMidPosition, jumpEndPosition;
	[SerializeField] float jumpInterpTime = 0;
	bool canJump = false;


	void Start()
	{
		// Offset idle animation clip
		if (animator != null)
		{
			AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
			animator.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
		}

		// Ensure the Rigidbody reference is set
		if (rb == null)
		{
			rb = GetComponent<Rigidbody>();
			if (rb == null)
			{
				rb = gameObject.AddComponent<Rigidbody>();
			}
		}

		rb.useGravity = false; // Initially disable gravity
		rb.isKinematic = true; // Set 
	}
	public void StartJumping(Vector3 jumpStartPos, Vector3 jumpMidPoint, Vector3 jumpEndPos)
	{
		jumpStartPosition = jumpStartPos;
		jumpMidPosition = jumpMidPoint;
		jumpEndPosition = jumpEndPos;
	}
	bool IsWithinStoppingDistance()
	{
		if (Hole != null)
			return (Hole.transform.position - rb.transform.position).sqrMagnitude < JumpDetectionRadius * JumpDetectionRadius;
		else return false;
	}
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
				//player.transform.DOJump(Hole.transform.position + Vector3.down * 1 + GetRandomDirectionalVector() * HoleRadius, 1.5f, 1, .7f).SetEase(Ease.InQuad).OnComplete(() =>
				//{
				//	CrowdAudioManager.PlayJumpSound();
				//	OnGridElementJumped?.Invoke(this);
				//});
				agent.enabled = false;
				canJump = true;
				StartJumping(
					agent.transform.position,
					Hole.transform.position + Vector3.up * 3 + GetRandomDirectionalVector() * HoleRadius,
					Hole.transform.position + Vector3.down  + GetRandomDirectionalVector() * HoleRadius);
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
		yield return null;
	}
	public bool HasReachedHole()
	{

		if (transform != null && Hole != null)
		{

			float distanceToHole = Vector3.Distance(transform.position, Hole.transform.position);
			return distanceToHole <= 0.5f;
		}

		return false;
	}
	public void MarkAsFilled()
	{
		IsFilled = true;
	}
	public void ResetFillStatus()
	{
		IsFilled = false;
	}

	public void ChangePlayerMaterial(Material material)
	{
		playerRenderer.sharedMaterial = material;
	}
	private Vector3 GetRandomDirectionalVector()
	{
		return new Vector3(Random.Range(-1f, 1f), Random.Range(-1, 1), Random.Range(-1f, 1f)).normalized;
	}
	public void MoveToPosition(Vector3 pipeMouthPosition)
	{
		targetPosition = transform.position;
		agent.transform.position = pipeMouthPosition;
		agent.transform.DOMove(targetPosition.Value, .5f).SetDelay(.2f).OnComplete(() =>
		{
			isRefilling = false;
			agent.transform.position = transform.position;
			agent.transform.forward = Vector3.back;
			animator.SetTrigger("Idle");
		});
		agent.enabled = false;
		agent.transform.forward = (targetPosition.Value - pipeMouthPosition).normalized;
		isRefilling = true;
		animator.SetTrigger("Run");
	}

	public void OnCrowdUpdate()
	{

		if (StartedRunning && IsWithinStoppingDistance())
		{
			StartCoroutine(OnReachedDestination());
		}
		if (canJump)
		{
			jumpInterpTime += Time.deltaTime ;
			agent.transform.position = VectorExt.CubicBezier(jumpStartPosition, jumpMidPosition, jumpEndPosition, jumpInterpTime);
			if (jumpInterpTime > 1)
			{
				jumpInterpTime = 1;
				canJump = false;
				CrowdAudioManager.PlayJumpSound();
				OnGridElementJumped?.Invoke(this);
			}
		}

		//if (targetPosition.HasValue)
		//{
		//	Vector3 distanceVector = targetPosition.Value - agent.transform.position;
		//	if (distanceVector.sqrMagnitude < 0.01f)
		//	{
		//		targetPosition = null;
		//		isRefilling = false;
		//		agent.transform.position = transform.position;
		//		agent.transform.forward = Vector3.back;
		//		animator.SetTrigger("Idle");
		//	}
		//	agent.transform.Translate(distanceVector.normalized);
		//}
		//else
		//{
		//	Debug.LogWarning("Target position is not set!");
		//}


	}
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawSphere(agent.transform.position, .2f);
		Gizmos.DrawSphere(jumpStartPosition, .2f);
		Gizmos.DrawSphere(jumpMidPosition, .2f);
		Gizmos.DrawSphere(jumpEndPosition, .2f);
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(VectorExt.CubicBezier(jumpStartPosition, jumpMidPosition, jumpEndPosition, jumpInterpTime), .2f);
	}
}
