using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public abstract class PopupWindow : MonoBehaviour
{
    CanvasGroup canvasGroup;

    float fadeDuration = .25f;

    public virtual void Init(string category, bool Unlocked = false, Sprite icon = null)
    {

    }
    public virtual void Open()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        gameObject.SetActive(true);
        canvasGroup.DOFade(1f, fadeDuration);
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
}
