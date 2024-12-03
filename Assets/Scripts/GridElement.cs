//using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using static Unity.VisualScripting.StickyNote;
using System.Collections;

public enum BlockedSide
{
    Up, Down, Left, Right, Null
}

public class GridElement : MonoBehaviour
{
    public Color gizmoColor = Color.yellow;
    public int Row;
    public int Column;
    public bool BlockedPath;
    public bool IsOccupied;
    public bool Isempty;
    public ColorEnum PlayerColor;
    public GameObject Player;
    public SkinnedMeshRenderer playerRenderer;
    public NavMeshAgent agent;
    public Animator animator;
    //public GameObject PlayerLeg;
    //public GameObject PlayerHead;
    // public GameObject PlayerHeadOutLine;
    //  public Animator Animator;
    // public Material SleepingMaterial;
    //public Material PlayerAcivatedMaterial;
    //public Material PlayerDeAcivatedMaterial;
    //  public bool blockSides;
    //  public BlockedSide BlockedSides;
    // public GameObject Block;
    public Vector3 PlayerInitialPos;
    public Vector3 PlayerInitialScale;

    public Rigidbody rb;
    // public bool Activated;
    //  public SpriteRenderer spriteRenderer;
    //  public GameObject RingParticle;
    // public DOTweenAnimation DOTweenAnimation;


    private void Awake()
    {
        PlayerInitialPos = Player.transform.localPosition;
        PlayerInitialScale = Player.transform.localScale;
        agent.enabled = false;
        // DOTweenAnimation.DOPause();
        //DOTweenAnimation.enabled = false;
    }
    void Start()
    {
        rb.useGravity = false;

        //if (BlockedPath)
        //{
        //    if (HelpManager.instance.path.Contains(this.gameObject))
        //    {
        //        HelpManager.instance.path.Remove(this.gameObject);
        //    }
        //}
    }
    private void OnDrawGizmos()
    {

        Gizmos.color = gizmoColor;
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnValidate()
    {
        //if (BlockedSides == BlockedSide.Null)
        //{
        //   // Block.SetActive(false);
        //    blockSides = false;
        //    Player.SetActive(true);
        //}
        //else if (BlockedSides == BlockedSide.Left)
        //{
        //   // Block.SetActive(true);
        //    blockSides = true;
        //    Player.SetActive(false);
        //  //  Block.transform.rotation = Quaternion.Euler(0, 90, 0);
        //}
        //else if (BlockedSides == BlockedSide.Right)
        //{
        //   // Block.SetActive(true);
        //    blockSides = true;
        //    Player.SetActive(false);
        //   // Block.transform.rotation = Quaternion.Euler(0, 270, 0);
        //}
        //else if (BlockedSides == BlockedSide.Up)
        //{
        //  //  Block.SetActive(true);
        //    blockSides = true;
        //    Player.SetActive(false);
        //    //Block.transform.rotation = Quaternion.Euler(0, 180, 0);
        //}
        //else
        //{
        //    //Block.SetActive(true);
        //    blockSides = true;
        //    Player.SetActive(false);
        //   // Block.transform.rotation = Quaternion.Euler(0, 0, 0);
        //}

        if (BlockedPath)
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }

            // Deactivate all children
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        //if (IsSleepingPlayer)
        //{
        //    changePlayerMaterial(SleepingMaterial);
        //}
    }





    public void changePlayerMaterial(Material material)
    {
        //// Renderer LegRenderer = PlayerLeg.GetComponent<Renderer>();
        // if (LegRenderer != null)
        // {
        //     LegRenderer.material = material;
        // }
        // Renderer HeadRenderer = PlayerHead.GetComponent<Renderer>();
        // if (HeadRenderer != null)
        // {
        //     HeadRenderer.material = material;
        // }
        //Renderer HeadRendererLine = PlayerHeadOutLine.GetComponent<Renderer>();
        //if( HeadRendererLine != null)
        //{
        //    HeadRendererLine.material = material;
        //}
    }

    //public void PlayDoAnim()
    // {
    //     Player.transform.DOKill();
    //    Player.transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.7f, 6, 0.141f)
    //     .SetLoops(-1, LoopType.Restart).OnStart(() =>
    //     {
    //         Debug.Log("DO ANIM PLAYER--"+transform.gameObject.name);
    //     }); // Infinite looping with a Restart effect
    //     Debug.Log("DO ANIM PLAYER--" + transform.gameObject.name);

    // }
    //public IEnumerator JumpToHole()
    //{

    //    if (agent.remainingDistance <=agent.stoppingDistance)
    //    {
    //        // Stop the agent and play the jump/fall animation
    //        if (agent.velocity.magnitude <= 0.1f) // If the agent is practically stopped
    //        {
    //            // Trigger jump/fall animatior
    //            if (animator != null)
    //            {
    //                animator.SetTrigger("Jump"); // Assuming you have a "Jump" trigger in your Animator
    //            }

    //            // Optionally, you can disable the agent after the jump/fall animation is triggered
    //            agent.enabled = false;
    //        }
    //    }
    //    else
    //    {
    //        yield return new WaitForSeconds(0.1f);
    //        StartCoroutine(JumpToHole());
    //    }
    //}


    public IEnumerator JumpToHole()
    {
        // Wait for the agent to reach the destination
        while (agent.remainingDistance > agent.stoppingDistance || agent.velocity.magnitude > 0.1f)
        {
            yield return null; // Wait for the next frame
        }

        // Agent has reached the destination and stopped
        if (animator != null)
        {
            animator.SetTrigger("Jump"); // Trigger the jump animation
        }

        // Optionally disable the agent while the jump animation plays
        agent.enabled = false;

        // Wait for the duration of the jump animation
        yield return new WaitForSeconds(1f); // Adjust this based on your animation duration

        // Re-enable the agent or perform follow-up actions if needed
        agent.enabled = true;
    }


    ////public IEnumerator JumpToHole()
    ////{
    ////    // Move to the hole destination using NavMeshAgent
    ////    if (agent.remainingDistance > agent.stoppingDistance)
    ////    {
    ////        yield return new WaitForSeconds(0.1f);
    ////        StartCoroutine(JumpToHole());
    ////    }
    ////    else
    ////    {
    ////        // Once the agent is close enough and practically stopped
    ////        if (agent.velocity.magnitude <= 0.1f) // Check if the agent is practically stopped
    ////        {
    ////            // Trigger the jump/fall animation
    ////            if (animator != null)
    ////            {
    ////                animator.SetTrigger("Jump"); // Assuming you have a "Jump" trigger in your Animator
    ////            }

    ////            // Optionally, you can disable the agent after the jump/fall animation is triggered
    ////            agent.enabled = false;

    ////            // Optionally wait for the duration of the animation or a specific time before continuing
    ////            yield return new WaitForSeconds(1f); // Adjust the wait time based on your animation length

    ////            // After the animation is done, proceed with any follow-up logic here (falling down, etc.)
    ////            // You can re-enable the agent here if you need to continue movement or do other things.
    ////        }
    ////        else
    ////        {
    ////            // If the agent is still moving slightly, wait and check again in the next frame
    ////            yield return null;
    ////        }
    ////    }
    ////}

}

