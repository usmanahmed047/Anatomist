using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AspectRatioFitter)), ExecuteInEditMode()]
public class AspectRatioMaintainer : MonoBehaviour
{
    AspectRatioFitter fitter;

    [SerializeField]
    Image image;

    private void Update()
    {
        if (fitter == null)
            fitter = GetComponent<AspectRatioFitter>();

        if (image.sprite == null)
        {
            fitter.aspectRatio = 1f;
            return;
        }
        

        if (fitter == null) return;

        if (image == null) return;

        float ratio = image.preferredWidth / image.preferredHeight;

        fitter.aspectRatio = ratio;
    }
}
