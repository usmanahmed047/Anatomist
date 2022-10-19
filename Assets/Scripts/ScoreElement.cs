using UnityEngine;
using UnityEngine.UI;

public class ScoreElement : MonoBehaviour
{
    public Text rankText;
    public Text usernameText;
    public Text scoreText;

    public void NewScoreElement(int rank, string _username, int _score)
    {
        rankText.GetComponent<ArabicText>().enabled = false;
        rankText.text = rank.ToString();
        if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
        {
            rankText.GetComponent<ArabicText>().enabled = true;
            rankText.GetComponent<ArabicText>().Text = rankText.text;
            rankText.GetComponent<ArabicText>().Refresh();
        }

        usernameText.GetComponent<ArabicText>().enabled = false;
        usernameText.text = _username;
        if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
        {
            usernameText.GetComponent<ArabicText>().enabled = true;
            usernameText.GetComponent<ArabicText>().Text = usernameText.text;
            usernameText.GetComponent<ArabicText>().Refresh();
        }
        scoreText.GetComponent<ArabicText>().enabled = false;
        scoreText.text = _score.ToString();
        if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
        {
            scoreText.GetComponent<ArabicText>().enabled = true;
            scoreText.GetComponent<ArabicText>().Text = scoreText.text;
            scoreText.GetComponent<ArabicText>().Refresh();
        }

    }

}
