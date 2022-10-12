using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSV : MonoBehaviour {

    /// <summary>
    /// This function assumes the first row is a header row listing the names for each field type in all rows that come after it.
    /// </summary>
    public static List<Dictionary<string, object>> Parse(string csvText, char lineSeparator, char delimiter)
    {
        if (string.IsNullOrEmpty(csvText))
        {
            Debug.LogError("TEXT DID NOT LOAD FOR CSV PARSING");
            return null;
        }

        string[] lines = csvText.Replace("\r", "").Split(lineSeparator);

        List<Dictionary<string, object>> csvData = new List<Dictionary<string, object>>();

        
        string[] names = lines[0].Replace("\"", "").Split(delimiter);

        for (int x = 1; x < lines.Length; x++)
        {
            if (string.IsNullOrEmpty(lines[x]))
            {
                continue;
            }
            string[] vals = lines[x].Split(delimiter);

            if (vals.Length != names.Length)
            {
                throw new System.MissingFieldException("Line " + (x - 1).ToString() + " is missing 1 or more fields.");
            }

            Dictionary<string, object> lineData = new Dictionary<string, object>();
            int y = 0;
            foreach (string field in names)
            {

                try
                {
                    lineData.Add(field, vals[y].Replace("\"", ""));
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Import failed on line " + x + ", column " + y);
                    throw e;

                }
                y++;
            }

            csvData.Add(lineData);
        }


        return csvData;
    }

    /// <summary>
    /// Useful for converting the JSON back into a Class object if the structure is known and class is already defined.
    /// This function expects the CSV file to use \n for line breaks, and a pipe '|' for delimiting
    /// </summary>
    public static string ToJson(string text)
    {
        return JsonUtility.ToJson(Parse(text, '\n', '|'));
    }

    /// <summary>
    /// Useful for converting the JSON back into a Class object if the structure is known and class is already defined.
    /// </summary>
    public static string ToJson(List<Dictionary<string, object>> data)
    {
        return JsonUtility.ToJson(data);
    }
}
