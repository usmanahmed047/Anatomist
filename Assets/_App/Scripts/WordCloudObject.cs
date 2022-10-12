using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WordCloudObject : MonoBehaviour
{
    float xStartPos = 0f;

    public void Awake()
    {
        xStartPos = transform.position.x;
    }

    float positiveOrNegative = 1f;

    public void OnEnable()
    {
        if (DOTween.IsTweening(transform))
            DOTween.Kill(transform);
        DOVirtual.DelayedCall(0.2f, () =>
        {
            transform.position = new Vector3(xStartPos, transform.position.y, transform.position.z);

            int rand = Random.Range(0, 100);
            if (rand >= 50)
                positiveOrNegative = -1f;

            transform.DOMoveX(xStartPos + (25f * positiveOrNegative), Random.Range(4f, 14f)).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);


            rand = Random.Range(0, 100);

            if (rand >= 50)
            {
                transform.DOScale(0.8f, Random.Range(4f, 14f)).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                transform.DOScale(1.2f, Random.Range(4f, 14f)).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);
            }
            transform.DOShakePosition(90f, 0.2f);
        });
    }


}
