using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerUpHandler
{
    string buttonSound = "swipe";

    public void OnPointerUp(PointerEventData eventData)
    {
        MusicManager.Instance.PlaySound(buttonSound);
    }
}
