using UnityEngine;

[CreateAssetMenu(fileName = "CroudManagerData", menuName = "ScriptableObjects/CroudManagerData", order = 1)]
public class CroudManagerData : ScriptableObject
{
	public GameObject croudPrefab;
	public float moveDuration = 1.0f;
	[SerializeField] string runTriggerName = "Run";
	[SerializeField] string jumpTriggerName = "Jump";
	public ColorManager colorManager;
	public Vector2 spacing;
	[Tooltip("the time interval at which crowd will Updates")]
	public float crowdUpdateInterval = 0.1f;
	private int runID;
	private int jumpId;

	public int RunId { get { if (runID == 0) runID = Animator.StringToHash(runTriggerName); return runID; } }
	public int JumpId { get { if (jumpId == 0) jumpId = Animator.StringToHash(jumpTriggerName); return jumpId; } }



}



