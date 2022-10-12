using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class DataImporter : EditorWindow {

    [MenuItem("Tools/Import Quiz Data (Multilingual)")]
    static void Init()
    {
        DataImporter window = GetWindow<DataImporter>(true, "Quiz Data Importer");
        window.ShowUtility();
    }

    List<Dictionary<string, object>> data;

    string json;

    string quizDataOutputFolder;

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Import CSV File: ");
        if (GUILayout.Button("GO"))
        {
            data = null;

            TextAsset ta = Resources.Load<TextAsset>("QuizDataALL");
            Debug.LogWarning("TEXT CONTENTS: " + ta.text);

            data = CSV.Parse(ta.text, '\n', '|');
            json = CSV.ToJson(data);
            Debug.Log(CSV.ToJson(data));
        }


        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();

        if (data != null)
        {
            EditorGUILayout.LabelField("Data = " + data.ToString() + "\n " + "Count = " + data.Count.ToString());
        }
        else
        {
            EditorGUILayout.LabelField("Data = null");
        }


        if (!string.IsNullOrEmpty(json))
        {
            EditorGUILayout.LabelField("JSON = " + "\n" + json);
        }
        else
        {
            EditorGUILayout.LabelField("JSON = null");
        }

        if (data != null && data.Count > 0)
        {

            if (GUILayout.Button("Process Files"))
            {
                ProcessLanguages(data);
            }


            int i = 0;
            foreach (Dictionary<string, object> dict in data)
            {
                foreach (KeyValuePair<string, object> kvp in dict)
                {
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value));

                    EditorGUILayout.EndHorizontal();
                    i++;

                    if (i >= 25)
                        break;
                }

                if (i >= 25)
                    break;
            }
        }

        EditorGUILayout.EndVertical();
    }

    AnatomistGameData gameData;

    void LoadReferenceData()
    {
        TextAsset ta = Resources.Load<TextAsset>("QuizData-en");

        gameData = JsonUtility.FromJson<AnatomistGameData>(ta.text);
    }

    public class LANGUAGECSV
    {
        public string lang;
        public string tag;
    }

    [System.Serializable]
    public class MissingQuestion
    {
        public string classification;
        public string answer;
        public string question;

        public MissingQuestion(string classification, string answer, string question)
        {
            this.classification = classification;
            this.answer = answer;
            this.question = question;
        }
    }

    void ProcessLanguages(List<Dictionary<string, object>> allData)
    {
        //language is something like SPANISH that we use to find the columns in the CSV data, tag is what we use for the QuizData-tag.txt file where tag will be something like es or zh-Quan

        //Reload the reference data for each language we process to make sure we are always referencing the English version of the data for lookup of field names.
        LoadReferenceData();

        AnatomistGameData newGameData = gameData;
        string missingQuestions = "";
        foreach (Dictionary<string, object> row in allData)
        {
            bool isPathology = ((string)row["SEARCH_ENGLISH"]).ToUpper().Contains("PATHOLOGY") || ((string)row["SOURCE IMAGE"]).ToUpper().Contains("PATHOLOGY");
            //First we dig into the proper category
            QuizCategory cat = newGameData.categories.Find(x => x.name.ToUpper().Trim() == ((string)row["SECTION"]).ToUpper().Trim());

            if (cat == null)
            {
                missingQuestions += "CATEGORY NOT FOUND: " + ((string)row["SECTION"]) + "\n";
                continue;
            }

            //if (isPathology)
            //    continue;
            //Add cases here to first check and see if the question is actually a pathology question we are searching for
            //if it is a pathology question, use q.pathology == true to check we found the right question
            //Remove the " Pathology" text from the classification before assigning it.

            

           
            string c = ((string)row["SEARCH_ENGLISH"]).Trim();
            string a = ((string)row["QUIZ_ENGLISH"]).Trim();
            string qq = ((string)row["SOURCE IMAGE"]).Trim();
            QuizQuestion q = cat.questions.Find(x => x.ClassificationText == c && x.AnswerText == a && x.QuestionText == qq && x.isPathology == isPathology);
           


            if (q == null)
            {
                //Find the reason why the lookup failed

                q = cat.questions.Find(x => x.ClassificationText == c);
                //Did it find it when we searched for JUST the classification?
                bool foundClassification = q != null;

                q = cat.questions.Find(x => x.AnswerText == a);
                //Did it find it when we searched for JUST the classification?
                bool foundAnswer = q != null;

                q = cat.questions.Find(x => x.ClassificationText == c && x.AnswerText == a);
                //Did it find it when we searched for Classificatin AND Answer?
                bool foundClassificationAndAnswer = q != null;

                if (foundClassificationAndAnswer && q.QuestionText != qq)
                {
                    //Source image wasnt found
                    missingQuestions += (isPathology ? "PATHOLOLGY" : "") + " SOURCE IMAGE NOT FOUND: " + qq + "\n" + q.QuestionText + "\n\n";
                }
                else if (foundClassification && !foundAnswer)
                {
                    missingQuestions += (isPathology ? "PATHOLOLGY" : "") + " ANSWER NOT FOUND: " + a + "\n";
                }
                else
                {
                    missingQuestions += (isPathology ? "PATHOLOLGY" : "") + " CLASSIFICATION NOT FOUND: " + c + "\n";
                }
                //missingQuestions += "Classification: " + (string)row["SEARCH_ENGLISH"] + "Answer: " + (string)row["QUIZ_ENGLISH"] + "Question: " + (string)row["SOURCE IMAGE"] + "\n";
                continue;
            }
            else
            {
                Debug.Log("QUESTION: " + q);
                Debug.Log("SUBCAT: " + q.subCategory);
                Debug.Log("NEW SUBCAT DATA: " + (string)row["SUBSECTION_ENGLISH"]);
                q.subCategory = (string)row["SUBSECTION_ENGLISH"];


                q.classification_SPANISH = (string)row["SEARCH_SPANISH"];
                q.answer_SPANISH = (string)row["QUIZ_SPANISH"];
                q.subCategory_SPANISH = (string)row["SUBSECTION_SPANISH"];

                q.classification_RUSSIAN = (string)row["SEARCH_RUSSIAN"];
                q.answer_RUSSIAN = (string)row["QUIZ_RUSSIAN"];
                q.subCategory_RUSSIAN = (string)row["SUBSECTION_RUSSIAN"];

                q.classification_ITALIAN = (string)row["SEARCH_ITALIAN"];
                q.answer_ITALIAN = (string)row["QUIZ_ITALIAN"];
                q.subCategory_ITALIAN = (string)row["SUBSECTION_ITALIAN"];

                q.classification_CHINESE = (string)row["SEARCH_CHINESE"];
                q.answer_CHINESE = (string)row["QUIZ_CHINESE"];
                q.subCategory_CHINESE = (string)row["SUBSECTION_CHINESE"];

                q.classification_FRENCH = (string)row["SEARCH_FRENCH"];
                q.answer_FRENCH = (string)row["QUIZ_FRENCH"];
                q.subCategory_FRENCH = (string)row["SUBSECTION_FRENCH"];

                q.classification_ARABIC = (string)row["SEARCH_ARABIC"];
                q.answer_ARABIC = (string)row["QUIZ_ARABIC"];
                q.subCategory_ARABIC = (string)row["SUBSECTION_ARABIC"];


                if (q.type == QuizType.Image)
                {
                    q.question_SPANISH = q.question_RUSSIAN = q.question_ITALIAN = q.question_FRENCH = q.question_CHINESE = q.question_ARABIC = q.QuestionText;
                }
                else
                {
                    //I dont think we have any text questions in this CSV file... so gonna leave this blank for now.
                }
            }
            
    
        }

        if (!string.IsNullOrEmpty(missingQuestions))
        {
            
            Debug.LogError("MISSING QUESTIONS: " + missingQuestions);

            StreamWriter sw1 = new StreamWriter(Application.dataPath + "/_App/Resources/IMPORTERRORS.txt", false, Encoding.UTF8, Encoding.UTF8.GetBytes(missingQuestions).Length);
            sw1.Write(missingQuestions);
            sw1.Close();
        }

        string quizJSON = JsonUtility.ToJson(newGameData, true);

        StreamWriter sw = new StreamWriter(Application.dataPath + "/_App/Resources/TESTQUIZDATA.txt", false, Encoding.UTF8, Encoding.UTF8.GetBytes(quizJSON).Length);
        sw.Write(quizJSON);
        sw.Close();

    }
}
