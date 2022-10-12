using GameSparks.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


public class LegacyData : MonoBehaviour {

    [ContextMenu("LoadAndConvertLegacyData")]
    public void LoadAndConvertLegacyData()
    {
        TextAsset ta = Resources.Load<TextAsset>("Anatomist");
        OldQuizData oqa = JsonUtility.FromJson<OldQuizData>(ta.text);

        AnatomistGameData gameData = new AnatomistGameData
        {
            categories = new List<QuizCategory>(),
            images = new List<QuizImage>()
        };


        //

        /*======================================================================
         * 
         *  Build the initial set of questions from the Anatomist file first
         * 
         ======================================================================*/

        for (int i = 0; i < oqa.oldQuizData.Length; i++)
        {
            OldQuizObject obj = oqa.oldQuizData[i];

            QuizCategory cat = gameData.categories.FirstOrDefault(x => x.name == obj.category);

            if (cat == null)
            {
                cat = new QuizCategory()
                {
                    name = obj.category,
                    questions = new List<QuizQuestion>()
                };

                gameData.categories.Add(cat);
            }


            QuizQuestion q = new QuizQuestion()
            {
                answer = obj.quiz,
                classification = obj.subcategory,
                pin = new QuizImagePin() { x = (float)obj.x, y = (float)obj.y },
                isPathology = string.IsNullOrEmpty(obj.pathology) ? false : true,
                question = obj.imageName,
                type = QuizType.Image,
                wikipediaUrl = ""
            };

            cat.questions.Add(q);


        }




        /*======================================================================
         * 
         *      NOW LOAD AND PROCESS THE OLD MEDICINE DATA
         * 
         ======================================================================*/

        ta = Resources.Load<TextAsset>("Medicine");
        OldMedicineData oma = JsonUtility.FromJson<OldMedicineData>(ta.text);

        foreach (OldMedicineObject obj in oma.oldMedicineData)
        {
            QuizCategory cat = gameData.categories.FirstOrDefault(x => x.name == "Medicine");

            if (cat == null)
            {
                cat = new QuizCategory()
                {
                    name = "Medicine",
                    questions = new List<QuizQuestion>()
                };

                gameData.categories.Add(cat);
            }

            QuizQuestion q = new QuizQuestion()
            {
                answer = obj.MedicineName,
                classification = obj.Classification,
                pin = null,
                isPathology = false,
                wikipediaUrl = obj.WikipediaURL,
                question = obj.TextParagraph,
                type = QuizType.Text
            };

            cat.questions.Add(q);
        }


        /*======================================================================
         * 
         *      NOW LOAD SOURCES AND CREATE THE IMAGE DATA
         * 
         ======================================================================*/

        ta = Resources.Load<TextAsset>("Source");
        OldSourceData osa = JsonUtility.FromJson<OldSourceData>(ta.text);


        foreach (OldSourceObject obj in osa.oldSrcData)
        {

            QuizImage img = new QuizImage()
            {
                imageName = obj.name,
                source = obj.source
            };

            gameData.images.Add(img);
        }


        /*======================================================================
         * 
         *      Now that we have all the new data converted and loaded
         *      we need to convert the x,y positions for each image question
         *      into percentage coords instead of pixel coords
         * 
         ======================================================================*/

        foreach (QuizCategory cat in gameData.categories)
        {
            foreach (QuizQuestion q in cat.questions)
            {
                if (q.pin != null && q.type == QuizType.Image)
                {
                    Texture texture = Resources.Load<Texture>("jpg/" + q.QuestionText.Replace(".jpg", ""));
                    if (texture != null)
                    {
                        Vector2 coords = ConvertImageCoordinates_PixelToPercentage(texture, new Vector2(q.pin.x, q.pin.y));

                        if (coords.x > 1f || coords.y > 1f) Debug.LogWarning(q.ClassificationText + " has an issue with its pin location... ?");

                        q.pin.x = coords.x;
                        q.pin.y = coords.y;
                    }
                    else
                    {
                        Debug.LogError("QUIZ IMAGE NOT FOUND: " + q.QuestionText);
                    }

                    texture = null;

                    Resources.UnloadUnusedAssets();
                }
            }
        }

        Resources.UnloadUnusedAssets();
        /*======================================================================
         * 
         *      TAKE ALL THE NEWLY CONVERTED DATA AND WRITE IT OUT TO JSON
         * 
         ======================================================================*/


        string newDataJson = JsonUtility.ToJson(gameData, true);


        string[] jsonLines = newDataJson.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // The using statement automatically flushes AND CLOSES the stream and calls 
        // IDisposable.Dispose on the stream object.
        using (StreamWriter file = new StreamWriter(Application.dataPath + "/_App/Resources/QuizData.txt"))
        {
            foreach (string line in jsonLines)
            {
                file.WriteLine(line);
            }
        }
    }

    public Vector2 ConvertImageCoordinates_PixelToPercentage(Texture texture, Vector2 pixelCoords)
    {
        float x = pixelCoords.x / texture.width;
        float y = pixelCoords.y / texture.height;

        return new Vector2(x, y);
    }
}
