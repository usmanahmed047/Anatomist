using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;


namespace Anatomist
{

    [RequireComponent(typeof(CanvasGroup)), System.Serializable]
    public abstract class Screen : MonoBehaviour
    {

        public static Screen current;

        
        public static Screen Find(string name)
        {
            Screen scr = ScreenManager.Instance.screens.FirstOrDefault(x => x.screenName.ToUpper() == name.ToUpper());

            if (scr == null)
                Debug.LogWarning(name + " Screen NOT FOUND");
            else
                Debug.LogWarning("Found screen: " + name);

            return scr;
        }


        public string screenName;

        CanvasGroup canvasGroup;
        float screenFadeDuration = .25f;
        

        [ContextMenu("Open")]
        public virtual void Open(params object[] args)
        {
            if (Screen.current == this) return; 

            Debug.LogWarning("Opening screen: " + gameObject.name + " with " + (args == null ? "0" : args.Length.ToString()) + " arguments." );

            if(canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

            if(current != null) current.Close();

            canvasGroup.alpha = 0f;
            gameObject.SetActive(true);

            canvasGroup.DOFade(1f, screenFadeDuration);

            current = this;
            
        }

        public virtual void Close()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

            canvasGroup.DOFade(0f, screenFadeDuration).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }

        public virtual void CloseImmediate()
        {
            gameObject.SetActive(false);
        }
    }
}