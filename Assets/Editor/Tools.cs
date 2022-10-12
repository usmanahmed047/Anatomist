using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace KBGameDev
{
    public class Tools : MonoBehaviour
    {
        [MenuItem("Tools/Delete PlayerPrefs")]
        public static void DeletePlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }



    }
}