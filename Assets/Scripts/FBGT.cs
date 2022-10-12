using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Firebase.Auth;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class FBGT : MonoBehaviour
{

    FirebaseAuth auth;
    public TMP_Text fb_userName;
    public Image fB_useerDp;
    private void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallBack, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }
    }
    private void InitCallBack()
    {
        if (!FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to initialize");
        }
    }
    private void OnHideUnity(bool isgameshown)
    {
        if (!isgameshown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void Facebook_Login()
    {
        List<string> permissions = new List<string>();

        permissions.Add("public_profile");

        permissions.Add("user_friends");

        FB.LogInWithReadPermissions(permissions, AuthCallBack);
       
    }

    private void AuthCallBack(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            //FBid.text = (aToken.UserId);

            string accesstoken;
            string[] data;
            string acc;
            string[] some;
#if UNITY_EDITOR
            Debug.Log("this is raw access " + result.RawResult);
            data = result.RawResult.Split(',');
            Debug.Log("this is access" + data[3]);
            acc = data[3];
            some = acc.Split('"');
            Debug.Log("this is access " + some[3]);
            accesstoken = some[3];
#elif UNITY_ANDROID
            Debug.Log("this is raw access "+result.RawResult);
            data = result.RawResult.Split(',');
            Debug.Log("this is access"+data[0]);
             acc = data[0];
             some = acc.Split('"');
            Debug.Log("this is access " + some[3]);


             accesstoken = some[3];
#endif
            authwithfirebase(accesstoken);
            DealWithFbMenus(FB.IsLoggedIn);
        }
        else
        {
            Debug.Log("User Cancelled login");
        }
    }
    public void authwithfirebase(string accesstoken)
    {
        auth = FirebaseAuth.DefaultInstance;
        Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(accesstoken);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("singin encountered error" + task.Exception);
            }
            Firebase.Auth.FirebaseUser newuser = task.Result;
            Debug.Log(newuser.DisplayName);
        });
    }
    void DealWithFbMenus(bool isLoggedIn)

    {

        if (isLoggedIn)

        {

            FB.API("/me?fields=first_name", HttpMethod.GET, DisplayUsername);

            FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
            SceneManager.LoadScene("MainMenu");

        }

        else

        {



        }

    }
    void DisplayUsername(IResult result)

    {

        if (result.Error == null)

        {

            string name = "" + result.ResultDictionary["first_name"];

            fb_userName.text = name;



            Debug.Log("" + name);

        }

        else

        {

            Debug.Log(result.Error);

        }

    }
    void DisplayProfilePic(IGraphResult result)

    {

        if (result.Texture != null)

        {

            Debug.Log("Profile Pic");

            fB_useerDp.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());

        }

        else

        {

            Debug.Log(result.Error);

        }

    }
}
