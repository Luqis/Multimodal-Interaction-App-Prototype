using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnButtonPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    SpeechListener instance = new SpeechListener();

    public void OnPointerDown(PointerEventData eventData)
    {
        instance.StartMicRecording();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        instance.StartSpeechRecording();
    }
}
