using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using PaperPlaneTools;

public class HyperlinkHandler : MonoBehaviour, IPointerClickHandler
{
    TextMeshProUGUI label;

    void Start()
    {
        label = GetComponent<TextMeshProUGUI>();

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (label == null) return;

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(label, eventData.position, eventData.pressEventCamera);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = label.textInfo.linkInfo[linkIndex];
            string url = linkInfo.GetLinkID();

            HandleLink(url);
        }
    }

    void HandleLink(string url)
    {
     
        switch (url)
        {
            case null:
            case "":
                break;
            case "https://play.google.com/store/apps/details?id=com.anatomist.app":
            case "itms-apps://itunes.apple.com/app/id1114940725":
                Debug.Log("Showing Rate App on App Store page");

                Application.OpenURL(url + "?action=write-review");
                break;
            case "https://www.instagram.com/andrewmeyerson/":
#if UNITY_IOS && !UNITY_EDITOR
                Application.OpenURL("instagram://user?username=andrewmeyerson");
#elif UNITY_ANDROID && !UNITY_EDITOR
                Application.OpenURL("instagram://user?username=andrewmeyerson");
#else
                Application.OpenURL(url);
#endif
                break;
            default:
                Application.OpenURL(url);
                break;
        }

    }

}
