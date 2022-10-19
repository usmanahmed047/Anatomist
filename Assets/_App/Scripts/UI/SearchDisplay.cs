using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Anatomist;

public class SearchDisplay : MonoBehaviour
{

    public static SearchDisplay Instance;
    public Image Display;
    public Image pin;
    public Sprite ImageSprite;
    public Text displayTxt;
    int PrevIndex;
    public SearchEntry CurrentEntry;
    // Use this for initialization
    void Awake()
    {
        Instance = this;
        SearchEntry.OnSearchResultClicked += InitiateImagePreview;
    }


    void Update()
    {
        CheckForSwiping();
    }

    enum SwipeDirection
    {
        None,
        Left,
        Right
    }

    SwipeDirection swipeDir = SwipeDirection.None;
    Vector2 startPosition;
    void CheckForSwiping()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    swipeDir = SwipeDirection.None;
                    startPosition = touch.position;
                    break;
                case TouchPhase.Moved:

                    if (Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y))
                    {
                        if (touch.deltaPosition.x < 0)
                        {
                            Debug.Log("Swiped Left");
                            swipeDir = SwipeDirection.Left;

                        }
                        else
                        {
                            Debug.Log("Swiped Right");
                            swipeDir = SwipeDirection.Right;
                        }
                    }

                    break;

                case TouchPhase.Ended:
                    //Make sure touch movement went at least 100 pixels to count as a swipe
                    if (Vector2.Distance(startPosition, touch.position) > 100f)
                    {
                        if (swipeDir == SwipeDirection.Left)
                            Next();
                        else if (swipeDir == SwipeDirection.Right)
                            Prev();
                    }

                    

                    break;
            }
        }
    }

    void InitiateImagePreview(SearchEntry searchEntry)
    {
        //int Index = 0;

        Debug.Log("Tex name: " + searchEntry.QuestionData.Texture.name);
        Texture2D tex = searchEntry.QuestionData.Texture;
        ImageSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        displayTxt.text = searchEntry.QuestionData.ClassificationText;

        displayTxt.GetComponent<ArabicText>().enabled = false;
        if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
        {
            displayTxt.GetComponent<ArabicText>().enabled = true;
            displayTxt.GetComponent<ArabicText>().Text = displayTxt.text;
            displayTxt.GetComponent<ArabicText>().Refresh();
        }
        Display.sprite = ImageSprite;//[Index];
        GameManager.Instance.AttachPinToImage(pin, Display, searchEntry.QuestionData);
        CurrentEntry = searchEntry;

        //Cache the search entry that is received
        //and use it in the Next() call to ask the search screen for the next one
        //and do the same for prev but you will need to write the function on the search screen for that


        //Use the question data to populate the text at the bottom of the screen with the classification.
    }

    public void Next()
    {
        SearchEntry ent = Screen_Search.Instance.GetNextSearchEntry(CurrentEntry);
        InitiateImagePreview(ent);
        //int Index = 0;
        //if (PrevIndex + 1 < PreviewSprite.Length)
        //    Index = PrevIndex + 1;
        //else
        //    Index = PrevIndex = 0;

        //Display.sprite = PreviewSprite[Index];
    }
    public void Prev()
    {
        SearchEntry ent = Screen_Search.Instance.GetPrevSearchEntry(CurrentEntry);
        InitiateImagePreview(ent);
        //int Index = 0;
        //if (PrevIndex + 1 < PreviewSprite.Length)
        //    Index = PrevIndex + 1;
        //else
        //    Index = PrevIndex = 0;

        //Display.sprite = PreviewSprite[Index];
    }
}
