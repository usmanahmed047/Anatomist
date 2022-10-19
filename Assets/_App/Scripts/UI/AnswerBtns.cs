using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using UnityEngine;
using UnityEngine.UI;

public class AnswerBtns : MonoBehaviour {

    Button btn;
    public Text txt;
    public bool IsAnswer;
    Image img;

    // Use this for initialization
    void Awake ()
    {
        btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(Answer);
        img = GetComponent<Image>();

    }
	public void UpdateAnswerBtn(bool IsAnswer,string txt)
    {
        //Debug.Log("SETTING UP BUTTON FOR ANSWER: " + IsAnswer.ToString() + " - " + txt);
        this.txt.text = txt;

        this.txt.GetComponent<ArabicText>().Text = this.txt.text;
        //this.txt.GetComponent<ArabicText>().Refresh();

        if (string.IsNullOrEmpty(this.txt.text))
            Debug.LogError("TEXT NOT SET ON ANSWER BUTTON: ", gameObject);
        this.IsAnswer = IsAnswer;
    }

    public void Answer()
    {
        if(IsAnswer)
            Anatomist.Screen_Quiz.Instance.CorrectAnswer(img);
        else
            Anatomist.Screen_Quiz.Instance.WrongAnswer(img);

    }
}
