using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource audioSource;
    public AudioClip jumpClip;
    public AudioClip wrongMoveClip;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Play(string clipName)
    {
        AudioClip clipToPlay = null;

        if (clipName == "Jump")
        {
            clipToPlay = jumpClip;
        }
        else if (clipName == "WrongMove")
        {
            clipToPlay = wrongMoveClip;
        }

        if (clipToPlay != null)
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.Audio;
//using UnityEngine;
//using System;

//public class AudioManager : MonoBehaviour
//{
//    public Sound[] sounds;

//    public static AudioManager instance;

//    private Dictionary<AudioClip, float> audioCooldowns = new Dictionary<AudioClip, float>();
//    private float defaultCooldown = 0.05f; // Default cooldown duration for custom audio

//    private void Awake()
//    {
//        if (instance == null)
//        {
//            instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//            return;
//        }

//        DontDestroyOnLoad(gameObject);

//        foreach (Sound s in sounds)
//        {
//            s.source = gameObject.AddComponent<AudioSource>();
//            s.source.clip = s.clip;

//            s.source.volume = s.volume;
//            s.source.pitch = s.pitch;
//            s.source.loop = s.loop;
//            s.source.spatialBlend = s.SpatialBlend;
//        }
//    }

//    private void Start()
//    {
//        Play("Theme");
//    }

//    public void Play(string name)
//    {
//        Sound s = Array.Find(sounds, sound => sound.name == name);
//        if (s == null)
//        {
//            Debug.LogWarning("Sound: " + name + " not found!");
//            return;
//        }

//        s.source.Play();
//    }

//    public void PlaySoundAtOnce(string name)
//    {
//        Sound s = Array.Find(sounds, sound => sound.name == name);
//        if (s == null)
//        {
//            Debug.LogWarning("Sound: " + name + " not found!");
//            return;
//        }

//        s.source.PlayOneShot(s.clip);
//    }

//    #region Custom Audio with Cooldown

//    public void PlayAudioWithCooldown(string name, float customCooldown = -1f)
//    {
//        Sound s = Array.Find(sounds, sound => sound.name == name);
//        if (s == null)
//        {
//            Debug.LogWarning("Sound: " + name + " not found!");
//            return;
//        }

//        float cooldown = customCooldown > 0 ? customCooldown : defaultCooldown;

//        if (!audioCooldowns.ContainsKey(s.clip) || Time.time >= audioCooldowns[s.clip])
//        {
//            s.source.PlayOneShot(s.clip); // Play the sound with OneShot
//            audioCooldowns[s.clip] = Time.time + cooldown; // Set cooldown for this clip
//        }
//    }

//    public void PlayBumpAudio()
//    {
//        PlayAudioWithCooldown("Bump", 0.05f); // Use the generic cooldown system for Bump audio
//    }

//    #endregion

//    #region Stop Audio

//    public void Stop(string name)
//    {
//        Sound s = Array.Find(sounds, sound => sound.name == name);
//        if (s == null)
//        {
//            Debug.LogWarning("Sound: " + name + " not found!");
//            return;
//        }

//        if (s.source.isPlaying)
//        {
//            s.source.Stop();
//        }
//    }

//    #endregion
//}
