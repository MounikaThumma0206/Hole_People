using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GridElement : MonoBehaviour
{
    public ColorEnum ElementColor;
    public ColorEnum GridColor;
    public int lineIndex;
    public PlayerGridGenerator playerGridGenerator;
    public Color gizmoColor = Color.yellow;
    public int Row;
    public int Column;
    public bool BlockedPath;
    public bool IsOccupied;
    public bool Isempty;
    public ColorEnum PlayerColor;
    public GameObject Player;
    public SkinnedMeshRenderer playerRenderer;
    public Rigidbody rb;
    public Animator animator;

    public bool IsFilled { get; private set; } = false;
    public Vector3 PlayerInitialPos;
    public Vector3 PlayerInitialScale;
    public bool StartedRunning;
    public GameObject Hole;

  

    void Start()
    {
        rb.useGravity = false;
    }
    public void MoveToHoleWithDOTween(GameObject hole)
    {
        if (Player != null && hole != null)
        {
            Vector3 holePosition = hole.transform.position;
            Player.transform.DOMove(holePosition, 1f).SetEase(Ease.InOutQuad).OnStart(() =>
            {
                if (animator != null)
                {
                    animator.SetTrigger("Jump");
                }
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.Play("Jump");
                }
            }).OnComplete(() =>
            {
                rb.useGravity = true;
                Player.SetActive(false);
                transform.gameObject.SetActive(false);
            });
        }
    }
    void Update()
    {
        if (StartedRunning)
        {
            if (IsWithinStoppingDistance())
            {
                StartCoroutine(OnReachedDestination());
            }
        }
    }

    bool IsWithinStoppingDistance()
    {

        return false;
    }


    private IEnumerator MovePlayersWithDelay(List<GridElement> playerGridElements, GameObject hole)
    {
        foreach (GridElement gridElement in playerGridElements)
        {
            gridElement.MoveToHoleWithDOTween(hole);
            yield return new WaitForSeconds(1f);
            StartCoroutine(PlayerGridGenerator.Instance.RemoveGridElementAfterDelay(gridElement, 0.5f));
        }
    }


    private IEnumerator OnReachedDestination()
    {
        StartedRunning = false;

        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }

        if (transform.childCount > 0)
        {
            GameObject player = transform.GetChild(0).gameObject;
            player.transform.parent = null;

            if (Hole != null)
            {
                player.transform.DOLocalMove(Hole.transform.localPosition, 0.3f).OnComplete(() =>
                {
                    rb.useGravity = true;
                });

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
}
