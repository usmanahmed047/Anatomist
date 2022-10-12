using GameSparks.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameQuizData
{
    public string Category;
    public string SubCategory;
    public QuizQuestion Question;
    public string[] WrongAnswers;

    public GameQuizData(string Category, string SubCategory, QuizQuestion Question, string[] WrongAnswers)
    {
        this.Category = Category;
        this.SubCategory = SubCategory;
        this.Question = Question;
        this.WrongAnswers = WrongAnswers;
    }
}

[System.Serializable]
public class LeaderboardEntry
{

    public string userId;
    public int score;
    public int totalScore;
    public string when;
    public string city;
    public string country;
    public string userName;
    public GSData externalIds;
    public int rank;
    public Texture2D Avatar;

    public LeaderboardEntry() { }
    public LeaderboardEntry(int rank, string userName, Texture2D Avatar, int score)
    {
        this.rank = rank;
        this.userName = userName;
        this.Avatar = Avatar;
        this.score = score;
    }
}





[System.Serializable]
public class AnatomistGameData
{
    public List<QuizCategory> categories;
    public List<QuizImage> images;
}

public enum QuizType
{
    Image,
    Text
}

[System.Serializable]
public class LocalizedCategoryNames
{
    public string en;
    public string es;
    public string fr;
    public string it;
    public string zhHans;
    public string ru;
    public string ar;
}


[System.Serializable]
public class QuizCategory
{
    public string name;

    public LocalizedCategoryNames localized;

    public string GetLocalizedName()
    {
        switch (LocalizationManager.CurrentLanguage.tag)
        {
            case "es":
                return localized.es;
                
            case "ru":
                return localized.ru;
            case "zh-Hans":
                return localized.zhHans;
            case "fr":
                return localized.fr;
            case "it":
                return localized.it;
            case "ar":
                return localized.ar;
            case "en":
            default:
                return localized.en;
        }
    }
    public List<QuizQuestion> questions;
    public Sprite CategorySprite;
}

[System.Serializable]
public class QuizQuestion
{
    /// <summary>
    /// Determines whether this question is an Image or Text
    /// </summary>
    public QuizType type;

    /// <summary>
    /// If type is set to Image, then question will contain the name of the image file  to load from the resources folder, otherwise it will contain a text question.
    /// </summary>
    public string question, question_SPANISH, question_FRENCH, question_ITALIAN, question_ARABIC, question_CHINESE, question_RUSSIAN;

    public string QuestionText
    {
        get
        {
            switch (LocalizationManager.CurrentLanguage.tag)
            {
                case "en":
                    return question;
                case "es":
                    return question_SPANISH;
                case "fr":
                    return question_FRENCH;
                case "it":
                    return question_ITALIAN;
                case "ar":
                    return question_ARABIC;
                case "zh-Hans":
                    return question_CHINESE;
                case "ru":
                    return question_RUSSIAN;
                default:
                    return question;
            }
        }
    }


    /// <summary>
    /// The answer that is used on the ONE acceptable answer button, which is randomly placed among the 4 choices.
    /// </summary>
    public string answer, answer_SPANISH, answer_FRENCH, answer_ITALIAN, answer_ARABIC, answer_CHINESE, answer_RUSSIAN;

    public string AnswerText
    {
        get
        {
            switch (LocalizationManager.CurrentLanguage.tag)
            {
                case "en":
                    return answer;
                case "es":
                    return answer_SPANISH;
                case "fr":
                    return answer_FRENCH;
                case "it":
                    return answer_ITALIAN;
                case "ar":
                    return answer_ARABIC;
                case "zh-Hans":
                    return answer_CHINESE;
                case "ru":
                    return answer_RUSSIAN;
                default:
                    return answer;
            }
        }
    }

    /// <summary>
    /// Classification is the text that is searchable in the Search screen, and is also used to gather a list of 3 wrong answers from other questions in the same category.
    /// </summary>
    public string classification, classification_SPANISH, classification_FRENCH, classification_ITALIAN, classification_ARABIC, classification_CHINESE, classification_RUSSIAN;

    public string ClassificationText
    {
        get
        {
            switch (LocalizationManager.CurrentLanguage.tag)
            {
                case "en":
                    return classification;
                case "es":
                    return classification_SPANISH;
                case "fr":
                    return classification_FRENCH;
                case "it":
                    return classification_ITALIAN;
                case "ar":
                    return classification_ARABIC;
                case "zh-Hans":
                    return classification_CHINESE;
                case "ru":
                    return classification_RUSSIAN;
                default:
                    return classification;
            }
        }
    }

    /// <summary>
    /// Contains the coordinates for where to place the pin on the image for this question.
    /// </summary>
    public QuizImagePin pin;

    /// <summary>
    /// Determines if this question only shows up in the pathology mode, or the normal modes.
    /// </summary>
    public bool isPathology;

    /// <summary>
    /// Only used for medicines at the moment, but should contain a link to the wikipedia article about the topic in this question.
    /// </summary>
    public string wikipediaUrl;


    public string subCategory = "", subCategory_GREEK, subCategory_SPANISH = "", subCategory_FRENCH = "", subCategory_ITALIAN = "", subCategory_ARABIC = "", subCategory_CHINESE = "", subCategory_RUSSIAN = "";


    public Texture2D Texture
    {
        get {
            Resources.UnloadUnusedAssets();

            Texture2D t = Resources.Load<Texture2D>("jpg/" + this.QuestionText.Replace(".jpg", "").Replace(".png", ""));

            if (t == null)
            {
                Debug.LogError("Could not load image: " + JsonUtility.ToJson(this, false));
            }

            return t;
        }
    }
}

[System.Serializable]
public class QuizImagePin
{
    public float x;
    public float y;
}

[System.Serializable]
public class QuizImage
{
    public string imageName;
    public string source;
}



#region OLD DATA TYPES

[System.Serializable]
public class OldQuizObject
{
    public string quiz;
    public string ID;
    public string category;
    public string imageName;
    public string pathology;
    public string subcategory;
    public int x;
    public int y;
}

[System.Serializable]
public class OldQuizData
{
    public OldQuizObject[] oldQuizData;
}

[System.Serializable]
public class OldMedicineData
{
    public OldMedicineObject[] oldMedicineData;
}

[System.Serializable]
public class OldMedicineObject
{
    /// <summary>
    /// category
    /// </summary>
    public string Classification;
    public string ID;

    /// <summary>
    /// the answer
    /// </summary>
    public string MedicineName;

    /// <summary>
    /// the question
    /// </summary>
    public string TextParagraph;


    public string WikipediaURL;
    public string Timestamp;
    /*
        "Classification": "Anaesthetics",
        "ID": "1",
        "MedicineName": "Halothane",
        "TextParagraph": "__________ is a general anaesthetic and can be used to start or maintain anaesthesia. (Inhaled)",
        "Timestamp": "12.22.2016 0:01:41",
        "WikipediaURL": "https://en.wikipedia.org/wiki/Halothane"
     */
}


[System.Serializable]
public class OldSourceData
{
    public OldSourceObject[] oldSrcData;
}

[System.Serializable]
public class OldSourceObject
{
    public string ID;
    public string image;
    public string name;
    public string source;
}

#endregion
