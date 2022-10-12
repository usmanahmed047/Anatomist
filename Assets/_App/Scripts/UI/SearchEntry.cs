using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Anatomist;

public class SearchEntry : MonoBehaviour
{
    public static event System.Action<SearchEntry> OnSearchResultClicked;
    public QuizQuestion QuestionData;

    //public Sprite[] Avatars;
    //int CurrentIndex;
	// Use this for initialization
	void Start ()
    {
        
	}
    

    public void OnButtonSelected()
    {
        Screen_Search.Instance.SearchDisplay.SetActive(true);
        OnSearchResultClicked?.Invoke(this);
    }

}
