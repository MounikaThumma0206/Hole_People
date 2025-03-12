using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
	[SerializeField] Toggle music, sfx, vibration;
	[SerializeField] AudioMixer audioMixer;
	private void Start()
	{
		// Load the settings from the PlayerPrefs
		LoadSettings();

		gameObject.SetActive(false);
	}


	private void OnEnable()
	{
		music.onValueChanged.AddListener((value) => { SetMusic(value); });
		sfx.onValueChanged.AddListener((value) => { SetSFX(value); });
		vibration.onValueChanged.AddListener((value) => { SetVibration(value); });
	}
	private void OnDisable()
	{
		music.onValueChanged.RemoveAllListeners();
		sfx.onValueChanged.RemoveAllListeners();
		vibration.onValueChanged.RemoveAllListeners();
		PlayerPrefs.SetInt("Music", music.isOn ? 1 : 0);
		PlayerPrefs.SetInt("SFX", sfx.isOn ? 1 : 0);
		PlayerPrefs.SetInt("Vibration", vibration.isOn ? 1 : 0);
		PlayerPrefs.Save();
	}
	private void LoadSettings()
	{

		bool value = PlayerPrefs.GetInt("Music", 1) == 1;
		SetMusic(music);
		music.isOn = value;

		value = PlayerPrefs.GetInt("SFX", 1) == 1;
		SetSFX(value);
		sfx.isOn = value;

		value = PlayerPrefs.GetInt("Vibration", 1) == 1;
		SetVibration(PlayerPrefs.GetInt("Vibration", 1) == 1);
		vibration.isOn = value;
	}

	private void SetVibration(bool value)
	{

	}

	private void SetSFX(bool value)
	{
		audioMixer.SetFloat("sfxVolume", value ? 0 : -80);
	}

	private void SetMusic(bool value)
	{
		audioMixer.SetFloat("musicVolume", value ? 0 : -80);
	}
}