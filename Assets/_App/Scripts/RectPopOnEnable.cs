using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RectPopOnEnable : MonoBehaviour
{

    RectTransform rt;


    Vector2 startSizeDelta;

    // Use this for initialization
    void Awake()
    {
        rt = GetComponent<RectTransform>();
        startSizeDelta = rt.sizeDelta;
    }

    private void OnEnable()
    {
        rt.sizeDelta = Vector2.zero;

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);

        DOVirtual.DelayedCall(transform.GetSiblingIndex()*.1f+.2f, () =>
        {
            rt.DOSizeDelta(startSizeDelta, .2f).OnComplete(() => 
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
            });
        });
    }

}
