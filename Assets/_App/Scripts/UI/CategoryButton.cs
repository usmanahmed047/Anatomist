using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour {

    public static event System.Action<string, Sprite> CategorySelected;

    public string category;
    void Awake()
    {
        GameManager.FinishedLoadingData += SpriteData;
    }
    public void OnSelected()
    {
        if (CategorySelected != null && !string.IsNullOrEmpty(category))
        {
            CategorySelected(category, GetComponent<Image>().sprite);        
        }
    }

    public void SpriteData()
    {
        if (!string.IsNullOrEmpty(this.category))
            GameManager.Instance.AssignSpriteToCategory(category, GetComponent<Image>().sprite);
    }
}
