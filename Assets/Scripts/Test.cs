using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    public GameObject EditorLogin;
    public GameObject google,facebook,twitter;

    public void Start()
    {
#if UNITY_EDITOR
        EditorLogin.SetActive(true);
        google.SetActive(false);
        facebook.SetActive(false);
        twitter.SetActive(false);
        SceneManager.LoadScene("MainMenu");
#else
        EditorLogin.SetActive(false);
        google.SetActive(true);
        facebook.SetActive(true);
        twitter.SetActive(true);
#endif
    }
}
