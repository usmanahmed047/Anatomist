using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GifPlayer : MonoBehaviour
{
    Image Display;
    public GifClass gifs;
    public float delay = 0.3f;
    float lastUpdateTime;
    int index;

    void Update()
    {
        if (Time.time - lastUpdateTime >= delay)
        {
            lastUpdateTime = Time.time;
            Display.sprite = gifs?.GifSprites[index];
            index++;
            if (index >= gifs.GifSprites.Length)
                index = 0;
        }
    }
    void OnEnable()
    {
        Display = GetComponent<Image>();
        index = 0;
    }


}
