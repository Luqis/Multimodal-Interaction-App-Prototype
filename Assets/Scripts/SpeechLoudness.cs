using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechLoudness : MonoBehaviour {

	public float sensitivity = 100f;
	public float loudness = 0;
	
	public RectTransform volumeBar;
	public AudioSource _audio;
	int currentMic = 0;
	int minFreqs;
	int maxFreqs;

	// Use this for initialization
	void Start () {

		Microphone.GetDeviceCaps(null, out minFreqs, out maxFreqs);
		_audio.loop = true;
		//If you pass a null or empty string for the device name then the default microphone will be used
		_audio.clip = Microphone.Start(null, true, 3, maxFreqs);
		_audio.Play();

	}

	public void RecordButtonPressed()
	{
		Microphone.GetDeviceCaps(Microphone.devices[currentMic], out minFreqs, out maxFreqs);

		//_audio.GetComponent<AudioSource>();
		_audio.loop = true;
		//If you pass a null or empty string for the device name then the default microphone will be used
		_audio.clip = Microphone.Start(Microphone.devices[currentMic], true, 3, maxFreqs);
	}
	
	// Update is called once per frame
	void Update () {

		loudness = GetAvgVolume() * sensitivity;
		if (loudness>1)
		{
			volumeBar.localScale = new Vector2(1f, loudness);
		}
	}

	public float GetAvgVolume()
	{
		float[] data = new float[256];
		float a = 0;
		_audio.GetOutputData(data, 0);
		foreach (float s in data)
		{
			a += Mathf.Abs(s);
		}
		return a / 256;
	}
}

