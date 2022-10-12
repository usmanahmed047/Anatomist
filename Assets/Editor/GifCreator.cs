using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public static class ScriptableObjectUtility
{
    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    public static T CreateAsset<T>(string filename) where T : ScriptableObject
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }


        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/ " + filename + ".asset");
        T asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, assetPathAndName);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        return asset;
    }

    [MenuItem("Assets/Create/GifObjects")]
    public static void CreateAsset()
    {
        string ScriptableObjectName = "";
        GifClass gc = null;
        for (int i = 1; i < 27; i++)
        {
            if (i < 10)
                ScriptableObjectName = "gif_0" + (i) + "_small";
            else
                ScriptableObjectName = "gif_" + (i) + "_small";

            gc = CreateAsset<GifClass>(ScriptableObjectName);
            GetSprites(gc);
        }
        ScriptableObjectName = "anat_pause_gif";
        gc = CreateAsset<GifClass>(ScriptableObjectName);
        GetSprites(gc);

        AssetDatabase.SaveAssets();
        
    }
    public static void GetSprites(GifClass gc)
    {
        gc.GifSprites = Resources.LoadAll<Sprite>(gc.name);

        ns.NumericComparer nc = new ns.NumericComparer();
        Array.Sort(gc.GifSprites, nc);
        EditorUtility.SetDirty(gc);
        AssetDatabase.SaveAssets();
    }
}



































//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;

//public class MakeScriptableObject
//{
//    [MenuItem("Assets/Create/GifObject")]
//    public static void CreateMyAsset()
//    {
//        GifClass asset = ScriptableObject.CreateInstance<GifClass>();
//System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Application.dataPath+ "/_App");
//int count = dir.GetFiles().Length;
//        List<string> FolderNames = new List<string>();
//        string ScriptableObjectName = "";
//        for (int i = 0; i < 26; i++) 
//        {
//            if (i > 0 && i < 10)
//            {
//                ScriptableObjectName = "gif_0" + (i) + "_small";
//                AssetDatabase.CreateAsset(asset, ScriptableObjectName + ".asset");
//                AssetDatabase.SaveAssets();

//                EditorUtility.FocusProjectWindow();

//                Selection.activeObject = asset;
//                // FolderNames.Add("gif_0" + (i) + "_small");
//            }
//            else if (i > 9)
//            {
//                ScriptableObjectName = "gif_" + (i) + "_small";
//                AssetDatabase.CreateAsset(asset, ScriptableObjectName + ".asset");
//                AssetDatabase.SaveAssets();

//                EditorUtility.FocusProjectWindow();

//                Selection.activeObject = asset;
//                //  FolderNames.Add("gif_" + (i) + "_small");
//            }
//        }

//      //  AssetDatabase.CreateAsset(asset, "Gifs/GifObject.asset");
//        AssetDatabase.SaveAssets();

//        EditorUtility.FocusProjectWindow();

//        Selection.activeObject = asset;
//    }
//}
