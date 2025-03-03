using UnityEngine;
public enum PillarType
{
	ACTIVE,
	DEACTIVE
}

public class Pillar : MonoBehaviour
{

	public PillarType PillarType=>owner.PillarType;

	[SerializeField] Animator animator;
	private int activeID;
	private CroudManager owner;
	private void Awake()
	{
		activeID = Animator.StringToHash("Active");
	}
	private void Start()
	{
		SwitchPillarType(ColorEnum.None);
	}
	public void SetOwner(CroudManager owner, PillarType pillarType)
	{
		this.owner = owner;
		SwitchPillarType(ColorEnum.None);
	}
	public void SwitchPillarType(ColorEnum colorEnum)
	{
		switch (PillarType)
		{
			case PillarType.ACTIVE:
				animator.SetBool(activeID, true);
				break;
			case PillarType.DEACTIVE:
				animator.SetBool(activeID, false);
				break;
		}
	}
}