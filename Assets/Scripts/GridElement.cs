using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class GridElement : MonoBehaviour
{
	public ColorEnum ElementColor;
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
	public AudioSource audioSource;
	public bool IsFilled { get; private set; } = false;
	public Vector3 PlayerInitialPos;
	public Vector3 PlayerInitialScale;
	public bool StartedRunning;
	public Hole Hole;
	public NavMeshAgent agent;
	private float HoleRadius => Hole.HoleRadius;
	private float JumpDetectionRadius => Hole.JumpDetectionRadius;
	void Start()
	{
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
	//public void MoveToHoleWithDOTween(GameObject hole)
	//{
	//	if (Player != null && hole != null)
	//	{
	//		Vector3 holePosition = hole.transform.position;
	//		Player.transform.DOMove(holePosition, 1f).SetEase(Ease.InOutQuad).OnStart(() =>
	//		{
	//			if (animator != null)
	//			{
	//				animator.SetTrigger("Jump");
	//			}
	//			if (AudioManager.instance != null)
	//			{
	//				AudioManager.instance.Play("Jump");
	//			}
	//		}).OnComplete(() =>
	//		{
	//			rb.useGravity = true;
	//			Player.SetActive(false);
	//			transform.gameObject.SetActive(false);
	//		});
	//	}
	//}
	void Update()
	{
		if (!StartedRunning)
		{
			return;
		}
		if (IsWithinStoppingDistance())
		{
			StartCoroutine(OnReachedDestination());
		}
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
				player.transform.DOMove(Hole.transform.position + Vector3.up * 2 + GetRandomDirectionalVector() * HoleRadius, 0.3f).SetEase(Ease.InQuad).OnComplete(() =>
				{
					player.transform.DOMove(Hole.transform.position + Vector3.down * 2 + GetRandomDirectionalVector() * HoleRadius, .3f).SetEase(Ease.InQuad);
					audioSource.Play();
				});

				//Vector3 holeDownPosition = Hole.transform.position + Vector3.down * 2 + GetRandomDirectionalVector() * HoleRadius;
				//player.transform.DOPath(new Vector3[]
				//{
				//	player.transform.position,//current position which is start
				//	Hole.transform.position + Vector3.up * 2 + GetRandomDirectionalVector() * HoleRadius,//this is the top position when the person jumps
				//	holeDownPosition //this is the bottom position of the hole
				//}, .8f, PathType.CubicBezier)
				//	.SetEase(Ease.InQuad).OnComplete(() =>{ player.transform.position = holeDownPosition;player.a });


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
	private void OnValidate()
	{
		if (BlockedPath)
		{
			BoxCollider boxCollider = GetComponent<BoxCollider>();
			if (boxCollider != null)
			{
				boxCollider.enabled = false;
			}
			foreach (Transform child in transform)
			{
				child.gameObject.SetActive(false);
			}
		}
	}

	public void changePlayerMaterial(Material material)
	{

	}
	private Vector3 GetRandomDirectionalVector()
	{
		return new Vector3(Random.Range(-1f, 1f), Random.Range(-1, 1), Random.Range(-1f, 1f)).normalized;
	}

}
