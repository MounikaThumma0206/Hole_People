using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CrowdAudioManager : MonoBehaviour
{
	public static CrowdAudioManager instance;
	[SerializeField] AudioSource audioSource;
	[SerializeField] AudioClip jumpClip;

	[SerializeField] float audioDelay = 0.1f;
	float lastPlayedTime;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
	public static void PlayJumpSound()
	{
		if (instance != null)
		{
			if (Time.time - instance.lastPlayedTime > instance.audioDelay)
			{
				instance.audioSource.PlayOneShot(instance.jumpClip);
				instance.lastPlayedTime = Time.time;
			}
		}
	}


}