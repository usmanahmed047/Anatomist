using Anatomist;
using Facebook.Unity;  //FOR FACEBOOK AUTH
using Firebase.Auth;
using Firebase.Database;
using Google;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterKit.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UserProfile = Firebase.Auth.UserProfile;

[Serializable]
public class GitAccessTokenRequest
{
    public string client_id;
    public string device_code;
    public string grant_type;

}

public class FirebaseAuthenticationsHandler : MonoBehaviour
{

    public const string googleSignIn = "googleSignIn";
    public const string facebookSignIn = "facebookSignIn";
    public const string twitterSignIn = "twitterSignIn";
    Screen_Leaderboard SL;

    public static FirebaseAuthenticationsHandler instance;
    public string GoogleWebAPI = "274946996410-8b8vknq91fl03qpt8t5k1u9nqbjm4mbk.apps.googleusercontent.com";  //Can be get from firebase console where we enable google authentication.

    public string git_client_id;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    GoogleSignInConfiguration configration;
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    public bool UserLoggedIn = false;
    public Text debugText;
    public DatabaseReference dbreference;

    //User Data variables
    [Header("UserData")]
    public string username;
    public GameObject scoreElement;


    public string selectedcategory;

    public Text GitAuthCode;
    public Button GitAccessTokenButton;
    public Button GitUrlButton;

    string userCode;
    string deviceCode;

    public string categoryToBeStored;
    public int scoreToBeStored;
    public string leaderboard_username;
    public string leaderboard_category;
    public int leaderboard_score;

    public GameObject loadingScreen;

    public string userId;
    private void Awake()
    {
        if (instance == null)
        {
            AwakeOnce();
            instance = this;
            DontDestroyOnLoad(this);

        }
        else
        {
            Destroy(this);
        }

    }
    private void AwakeOnce()
    {
        Twitter.AwakeInit();
    }

    // Start is called before the first frame update
    void Start()
    {
        configration = new GoogleSignInConfiguration
        {
            WebClientId = GoogleWebAPI,
            RequestIdToken = true,
            UseGameSignIn = false

        };
        Debug.Log("step 0");
        debugText.text = debugText.text + "   " + "step 0";

        InitFirebase();

        if (GetAutoLoginKeyValue(googleSignIn) == "true")
        {
            OnClickGoogleSignIn();
        }
        if (GetAutoLoginKeyValue(twitterSignIn) == "true")
        {
            OnClickTwitterSignin();
        }

    }

    void InitFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        dbreference = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("dbreference " + dbreference);
        FB.Init(InitCallBack);  //FOR FACEBOOK AUTH
        Twitter.Init();
        Debug.Log("step 0.1");
        debugText.text = debugText.text + "   " + "step 0.1";
        //Here is AutoLogin Function


    }

    //FOR FACEBOOK AUTH
    void InitCallBack()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
            Debug.Log("FB initialized");
            debugText.text = debugText.text + "   " + "FB initialized" + FB.IsLoggedIn;
        }
        else
        {

            Debug.Log("FB not initialized");
            debugText.text = debugText.text + "   " + "FB not initialized";
        }
        if (GetAutoLoginKeyValue(facebookSignIn) == "true")
        {
            OnClickFacebookSignin();
        }
    }

    public void Update()
    {
        if (UserLoggedIn)
        {
            username = user.DisplayName;
            UserLoggedIn = false;
            //SceneManager.LoadScene("MainMenu");
        }
    }

    public void OnClickGoogleSignIn()
    {
        //GoogleSignIn.Configuration = configration;
        // GoogleSignIn.Configuration.UseGameSignIn = false;
        // GoogleSignIn.Configuration.RequestIdToken = true;
        // GoogleSignIn.Configuration.RequestEmail = true;
        Debug.Log("step 1");
        debugText.text = debugText.text + "   " + "step 1";
        loadingScreen.SetActive(true);

        //GoogleSignIn.DefaultInstance.SignIn().ContinueWith(onGoogleAuthenticationFinished, TaskContinuationOptions.ExecuteSynchronously);






        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            UseGameSignIn = false,
            // Copy this value from the google-service.json file.
            // oauth_client with type == 3
            WebClientId = "274946996410-8b8vknq91fl03qpt8t5k1u9nqbjm4mbk.apps.googleusercontent.com"
        };

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();
        Debug.Log("Check if we need prompt here1");
        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();
        signIn.ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                signInCompleted.SetCanceled();
            }
            else if (task.IsFaulted)
            {
                signInCompleted.SetException(task.Exception);
            }
            else
            {
                Debug.Log("Check if we need to turn on prompt or not2");
                Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(((Task<GoogleSignInUser>)task).Result.IdToken, null);
                auth.SignInWithCredentialAsync(credential).ContinueWith(authTask =>
                {
                    if (authTask.IsCanceled)
                    {
                        signInCompleted.SetCanceled();
                    }
                    else if (authTask.IsFaulted)
                    {
                        signInCompleted.SetException(authTask.Exception);
                    }
                    else
                    {
                        Debug.Log("check when the result will be display");
                        signInCompleted.SetResult(((Task<FirebaseUser>)authTask).Result);

                        user = ((Task<FirebaseUser>)authTask).Result;
                        Debug.LogFormat("User signed in successfully: {0} ({1})",
                        user.DisplayName, user.UserId);
                        debugText.text = debugText.text + "   " + "User signed in successfully:" + user.DisplayName + user.UserId;

                        user = auth.CurrentUser;      //user's data like name dp can be taken from this variable.
                        UserLoggedIn = true;
                        username = user.DisplayName;
                        //call function for name
                        userId = user.UserId;
                        SaveKeyForAutoLogin(googleSignIn);
                        SceneManager.LoadScene("MainMenu");
                    }
                });
            }
        });
    }

    void onGoogleAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        Debug.Log("step 2");
        debugText.text = debugText.text + "   " + "step 2";
        //SceneManager.LoadScene("MainMenu");
        if (task.IsFaulted)
        {
            Debug.Log("Fault in login!");
        }
        else if (task.IsCanceled)
        {
            Debug.Log("Login canceled!");
        }
        else
        {
            Debug.Log("step 3");
            debugText.text = debugText.text + "   " + "step 3";
            GoogleSignin(task.Result.IdToken, null);

        }
    }

    void GoogleSignin(string googleIdToken, string googleAccessToken)
    {
        Firebase.Auth.Credential credential =
        Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                debugText.text = debugText.text + "   " + "SignInWithCredentialAsync was canceled.";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                debugText.text = debugText.text + "   " + "SignInWithCredentialAsync encountered an error: " + task.Exception;
                return;
            }

            user = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                user.DisplayName, user.UserId);
            debugText.text = debugText.text + "   " + "User signed in successfully:" + user.DisplayName + user.UserId;

            user = auth.CurrentUser;      //user's data like name dp can be taken from this variable.
            UserLoggedIn = true;
            username = user.DisplayName;

            // SceneManager.LoadScene("MainMenu");
        });
    }
    void CheckWithGoogleAutoLogin()
    {
        PlayerPrefs.GetString("googlelogin", "true");
    }


    //FOR FACEBOOK AUTH
    public void OnClickFacebookSignin()
    {
        Debug.Log("step f2");
        debugText.text = debugText.text + "   " + "step f2";
        var perms = new List<string>() {
            "public_profile",
            "email" };
        FB.LogInWithReadPermissions(perms, onFacebookLoginResult);
    }

    void onFacebookLoginResult(ILoginResult result)
    {
        Debug.Log("step f3");
        debugText.text = debugText.text + "   " + "step f3";
        if (FB.IsInitialized)
        {
            Debug.Log("step f4");
            debugText.text = debugText.text + "   " + "step f4";
            var accessToken = AccessToken.CurrentAccessToken;
            FacebookSignin(accessToken);
        }
        else
        {
            Debug.Log("Cancel login!");
            debugText.text = debugText.text + "   " + "Cancel login!" + FB.IsLoggedIn;
        }
    }

    void FacebookSignin(AccessToken accessToken)
    {
        Debug.Log("step f5");
        debugText.text = debugText.text + "   " + "step f5";
        Firebase.Auth.Credential credential =
        Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken.TokenString);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                debugText.text = debugText.text + "   " + "SignInWithCredentialAsync was canceled.";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                debugText.text = debugText.text + "   " + "SignInWithCredentialAsync encountered an error: " + task.Exception;
                return;
            }

            user = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
            UserLoggedIn = true;
            // SceneManager.LoadScene("MainMenu");
            debugText.text = debugText.text + "   " + "User signed in successfully: " + user.DisplayName + user.UserId;
            username = user.DisplayName;

            //call function for name
            userId = user.UserId;
            SaveKeyForAutoLogin(facebookSignIn);
            SceneManager.LoadScene("MainMenu");


        });
    }


    public void OnClickTwitterSignin()
    {
        Debug.Log("step t2");
        debugText.text = debugText.text + "   " + "step t2";
        Twitter.LogIn(TwitterLoginResult, (ApiError error) =>
        {
            Debug.Log(error.message);
            debugText.text = debugText.text + error.message;
        });
    }

    void TwitterLoginResult(TwitterSession session)
    {
        Debug.Log("step t3");
        debugText.text = debugText.text + "   " + "step t3";
        var accessToken = session.authToken.token;
        var secret = session.authToken.secret;
        TwitterSignin(accessToken.ToString(), secret.ToString());
    }

    void TwitterSignin(string accessToken, string secret)
    {
        Firebase.Auth.Credential credential =
        Firebase.Auth.TwitterAuthProvider.GetCredential(accessToken, secret);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            user = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
            user.DisplayName, user.UserId);
            username = user.DisplayName;
            //call function for name

            userId = user.UserId;
            SaveKeyForAutoLogin(twitterSignIn);
            SceneManager.LoadScene("MainMenu");

        });
    }

    public void OnClickGithubSignin()
    {
        //FOR WEB
        //var currentRequest = new RequestHelper
        //{
        //    Uri = "https://github.com/login/oauth/authorize?scrop=user:email&client_id=" + git_client_id,
        //    Headers = new Dictionary<string, string> {
        //        { "Authorization", "Other token..." }
        //    },
        //    Params = new Dictionary<string, string> {
        //        {"client_id" , "ccd571dd0da2643eff26" },
        //        { "redirect_uri" , "https://firtebaseauthentications.firebaseapp.com/__/auth/handler" },
        //        { "state" , "chnvkjfdhvb" },
        //        { "login" , "fatimaakhter3" },
        //        { "allow_signup" , "true" },
        //        { "code" , "" }
        //    }
        //};
        //var usersRoute = "https://github.com/login/oauth/authorize?scrop=user:email&client_id=" + git_client_id;
        //RestClient.Get(currentRequest).Then(res =>
        //{
        //    //Debug.Log(currentRequest.DownloadHandler.text);
        //    Debug.Log("git auth success" + res.Text);
        //    Application.OpenURL(usersRoute);

        //}).Catch(error =>
        //{
        //    Debug.Log("git auth error:" + error);
        //});
        //End FOR web

        //For device
        var currentRequest = new RequestHelper
        {
            Uri = "https://github.com/login/device/code",

            Params = new Dictionary<string, string> {
                {"client_id" , "947a9dce8bed816d0a90" },
                { "scope" , "repo,gist" }

            }
        };

        RestClient.Post(currentRequest).Then(res =>
        {
            //Debug.Log(currentRequest.DownloadHandler.text);
            Debug.Log("git auth success" + res.Text);
            string response = res.Text;
            string[] splitArray = response.Split(char.Parse("="));
            string[] deviceCodesplitArray = splitArray[1].Split(char.Parse("&"));
            string[] userCodesplitArray = splitArray[4].Split(char.Parse("&"));
            Debug.Log(deviceCodesplitArray[0]);
            Debug.Log(userCodesplitArray[0]);
            userCode = userCodesplitArray[0];
            deviceCode = deviceCodesplitArray[0];
            GitAuthCode.text = userCode;
            GitAuthCode.gameObject.SetActive(true);
            GitUrlButton.gameObject.SetActive(true);

            //Application.OpenURL(usersRoute);

        }).Catch(error =>
        {
            Debug.Log("git auth error:" + error);
        });
    }

    public void OnGitUrlClick()
    {
        Application.OpenURL("https://github.com/login/device");
        GitAccessTokenButton.gameObject.SetActive(true);
        GitAuthCode.gameObject.SetActive(false);
        GitUrlButton.gameObject.SetActive(false);
    }

    public void ClickToGetGitAccessToken()
    {
        RestClient.Post("https://github.com/login/oauth/access_token", new GitAccessTokenRequest
        {
            client_id = "947a9dce8bed816d0a90",
            device_code = deviceCode,
            grant_type = "urn:ietf:params:oauth:grant-type:device_code"

        }).Then(res =>
        {
            Debug.Log("git access token:" + res.Text);
            Debug.Log(res.GetType());
            string response = res.Text;
            string[] splitArray = response.Split(char.Parse("="));
            string[] splitArray2 = splitArray[1].Split(char.Parse("&"));
            Debug.Log(splitArray2[0]);
            GithubSignin(splitArray2[0]);
            GitAccessTokenButton.gameObject.SetActive(false);
        }).Catch(error =>
        {
            Debug.Log("git access token error:" + error);
        });
    }

    void GithubSignin(string accessToken)
    {
        Firebase.Auth.Credential credential =
            Firebase.Auth.GitHubAuthProvider.GetCredential(accessToken);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            user = task.Result;
            SceneManager.LoadScene("MainMenu");
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                user.DisplayName, user.UserId);
        });
    }


    public void SaveDataButton(string _category, int _score)
    {
        //Debug.Log("on save button click");
        UserLoggedIn = false;
        StartCoroutine(UpdateUsernameAuth(username));
        StartCoroutine(UpdateUsernameDatabase(username));

        //StartCoroutine(UpdateScore(_score));
        //StartCoroutine(UpdateCategory(_category));
        categoryToBeStored = _category;
        scoreToBeStored = _score;
        selectedcategory = _category;
    }

    public void OnScoreboardButton()
    {
        Debug.Log("on leaderboard button click");
        StartCoroutine(LoadScoreboardData());
    }

    private IEnumerator UpdateUsernameAuth(string _username)
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //Call the Firebase auth update user profile function passing the profile with the username
        var ProfileTask = user.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            Debug.Log("Auth username is now updated");
        }
    }


    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Set the currently logged in user username in the database
        var DBUsername = dbreference.Child("users").Child(user.UserId).Child("username").SetValueAsync(_username);
        Debug.Log(_username);
        yield return new WaitUntil(predicate: () => DBUsername.IsCompleted);

        if (DBUsername.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBUsername.Exception}");
        }
        else
        {
            StartCoroutine(UpdateCategory(categoryToBeStored));
            Debug.Log("Database username is now updated");
        }
    }

    private IEnumerator UpdateCategory(string _category)
    {
        //Set the currently logged in user kills
        //var DBCategory = dbreference.Child("users").Child(user.UserId).Child("username").Child("category").SetValueAsync(_category);
        var DBCategory = dbreference.Child("users").Child(user.UserId).Child("category").SetValueAsync(_category);
        Debug.Log(_category);
        yield return new WaitUntil(predicate: () => DBCategory.IsCompleted);

        if (DBCategory.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBCategory.Exception}");
        }
        else
        {
            StartCoroutine(UpdateScore(scoreToBeStored));
            Debug.Log("Category are now updated");
        }
    }

    private IEnumerator UpdateScore(int _score)
    {
        //Set the currently logged in user kills
        //var DBScore = dbreference.Child("users").Child(user.UserId).Child("username").Child("category").Child("score").SetValueAsync(_score);
        var DBScore = dbreference.Child("users").Child(user.UserId).Child("score").SetValueAsync(_score);

        yield return new WaitUntil(predicate: () => DBScore.IsCompleted);

        if (DBScore.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBScore.Exception}");
        }
        else
        {
            Debug.Log("Score are now updated");
        }
    }


    public void SaveKeyForAutoLogin(string key)
    {
        PlayerPrefs.SetString(key, "true");
    }

    public string GetAutoLoginKeyValue(string key)
    {
        return PlayerPrefs.GetString(key, "false");
    }

    //private IEnumerator LoadUserData()
    //{
    //    //Get the currently logged in user data
    //    var DBTask = dbreference.Child("users").Child(user.UserId).GetValueAsync();

    //    yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

    //    if (DBTask.Exception != null)
    //    {
    //        Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
    //    }
    //    else if (DBTask.Result.Value == null)
    //    {
    //        //No data exists yet
    //        scoreField.text = "0";
    //    }
    //    else
    //    {
    //        //Data has been retrieved
    //        DataSnapshot snapshot = DBTask.Result;
    //        scoreField.text = snapshot.Child("score").Value.ToString();
    //    }
    //}

    private IEnumerator LoadScoreboardData()
    {
        //Get all the users data ordered by kills amount
        Debug.Log("Load Scoreboard Data On ");
        var DBTask = dbreference.Child("users").OrderByChild("score").GetValueAsync();
        Debug.Log("Logs DB Task Completed " + DBTask.IsCompleted);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        Debug.Log("Logs DB Task Completed " + DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            Debug.Log("Before Snapshot");

            DataSnapshot snapshot = DBTask.Result;

            Debug.Log("Before Snapshot" + snapshot);

            //Destroy any existing scoreboard elements
            foreach (Transform child in UIManager.instance.scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            int i = 1;
            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                leaderboard_username = childSnapshot.Child("username").Value.ToString();
                leaderboard_category = childSnapshot.Child("category").Value.ToString();
                leaderboard_score = int.Parse(childSnapshot.Child("score").Value.ToString());
                if (childSnapshot.Key == userId)
                {
                    ScreenManager.Instance.screens[4].GetComponent<Screen_Leaderboard>().SetMYProperties(leaderboard_username, leaderboard_score.ToString());
                }
                GameObject scoreboardElement = Instantiate(scoreElement, UIManager.instance.scoreboardContent);
                scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(i, leaderboard_username, leaderboard_score);
                i++;

            }

            {

            }
            //Go to scoareboard screen
            //UserLoggedIn = false;
            UIManager.instance.Scoreboardpanel.SetActive(true);
            //Userdatapanel.SetActive(false);
        }
    }

    public void SignOut()
    {
        if (auth != null)
        {
            auth.SignOut();
            SceneManager.LoadScene("FirebaseAuthentication");
        }
    }
}
