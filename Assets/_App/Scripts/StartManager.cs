using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{

    [SerializeField]
    private CanvasGroup anatomistLogoGroup, gameSparksLogoGroup;


    private void Start()
    {
        anatomistLogoGroup.alpha = 0f;
        gameSparksLogoGroup.alpha = 0f;


        DOVirtual.DelayedCall(1f, () =>
        {
            gameSparksLogoGroup.DOFade(1f, .5f).OnComplete(() =>
            {

                DOVirtual.DelayedCall(1f, () =>
                {
                    gameSparksLogoGroup.DOFade(0f, .5f).OnComplete(() =>
                    {
                        anatomistLogoGroup.DOFade(1f, .5f);
                        SceneManager.LoadSceneAsync(1);
                    });
                });




                
            });
        });
    }
}
