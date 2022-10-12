using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class IPhoneXScalar : MonoBehaviour {

    [SerializeField]
    CanvasScaler scaler;

    [SerializeField]
    RectTransform safeArea;

    [SerializeField]
    float topOffset, bottomOffset;

    float ratio = 1f;

    private void Start()
    {
        ratio = (float)Screen.width / (float)Screen.height;

        bool isIPhoneX = false;

#if UNITY_IOS
        isIPhoneX = Device.generation == DeviceGeneration.iPhoneX;
#endif

        if ( (isIPhoneX || ratio <= 0.55f) && scaler != null && safeArea != null)
        {
            scaler.matchWidthOrHeight = 0f;
            safeArea.offsetMin = new Vector2(safeArea.offsetMin.x, bottomOffset);
            safeArea.offsetMax = new Vector2(safeArea.offsetMax.x, -topOffset);
        }
    }

    private void Update()
    {
        //Debug.Log("Ratio: " + ((float)Screen.width / (float)Screen.height).ToString());
    }
}
