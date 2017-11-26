using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class SpeechRecognition : MonoBehaviour
{
	public string[] keywords = new string[] { "up", "down", "left", "right" };
	public ConfidenceLevel confidence = ConfidenceLevel.Medium;
	public float speed = 1;

	public Text results;
	public Image target;

	protected PhraseRecognizer recognizer;
	protected string word = "right";

	private void Start()
	{
		if (keywords != null)
		{
			recognizer = new KeywordRecognizer(keywords, confidence);
			recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
			recognizer.Start();
		}
	}

	private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
	{
		word = args.text;
		results.text = "You said: <b>" + word + "</b>";
	}

	private void Update()
	{
		var x = target.transform.position.x;
		var y = target.transform.position.y;
		var z = target.transform.rotation.z;

		switch (word)
		{
		case "kanan pusing":
			z += speed;
			target.transform.Rotate (0, 0, -z, Space.World);
			break;
		case "down":
			y -= speed;
			break;
		case "left":
			x -= speed;
			break;
		case "right":
			x += speed;
			break;
		}

		target.transform.position = new Vector3(x, y, 0);
	}

	private void OnApplicationQuit()
	{
		if (recognizer != null && recognizer.IsRunning)
		{
			recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
			recognizer.Stop();
		}
	}
}