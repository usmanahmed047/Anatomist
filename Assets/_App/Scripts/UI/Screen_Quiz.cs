
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using PaperPlaneTools;

namespace Anatomist
{
    public class Screen_Quiz : Screen
    {
        public static Screen_Quiz Instance;

        [SerializeField]
        Text reviewListHeader;

        [SerializeField]
        Text question;
        [SerializeField]
        Text score;
        [SerializeField]
        Text GOscore;
        [SerializeField]
        Text bestScore, GOCorrectAnswers, CongratsTitle, NextButtonLabel;
        [SerializeField]
        Text bigDeal;
        [SerializeField]
        Text pause;
        [SerializeField]
        Text btn_ResumeQuiz;
        [SerializeField]
        Text btn_QuitQuiz;
        [SerializeField]
        Text TimeText;

        float Timer;
        int Score;
        bool Tick;

        public static bool timerEnabled = true;

        int BestScore
        {
            get
            {
                int x = PlayerPrefs.GetInt("BestScore_" + AllQuestions[0].Category + AllQuestions[0].SubCategory, 0);
                Debug.Log("Loading best score of: " + x);
                return x;
            }

            set
            {
                if (value > BestScore)
                {
                    Debug.LogWarning("NEW BEST SCORE: " + value + " > " + BestScore);
                    PlayerPrefs.SetInt("BestScore_" + AllQuestions[0].Category + AllQuestions[0].SubCategory, value);
                }
            }
        }

        //Quiz Info
        public Image QuestionImage;
        public GameObject[] AnswerBtns;
        public Text CorrectCountText;
        public Text WrongCountText;
        public GameObject PauseScreen;
        public Text QuestionText;
        int AnswerIndex;
        public GameQuizData[] AllQuestions;

        public List<QuizQuestion> WrongAnswers = new List<QuizQuestion>();
        public List<QuizQuestion> CorrectAnswers = new List<QuizQuestion>();

        int QuestionIndex;

        public GameObject GameOverComplete;
        public GameObject GameOverFailed;

        public Color CorrectColor;
        public Color WrongColor;
        public Color DefaultColor;
        public Color UnselectedColor;

        public Text SourceText;
        public GameObject Search;
        public GameObject CC;
        public Transform Content;
        public GameObject failedEntry;

        GameObject Correct;
        GameObject Wrong;

        public Transform ZoomImageNewParent;
        Transform ZoomImagePrevParent;
        public GameObject DisplayImage;
        public GameObject NewZoomedImage;
        public GameObject UnZoomButton;


        void Awake()
        {
            Instance = this;
        }

        bool isZoomed = false;

        public void Zoom()
        {
            //Tick = false;
            ZoomImagePrevParent = DisplayImage.transform.parent;

            NewZoomedImage = Instantiate(DisplayImage, ZoomImageNewParent);
            DisplayImage.SetActive(false);
            GameObject mainImage = null;
            foreach (Transform tr in NewZoomedImage.transform)
            {
                if (tr.name == "Button")
                {
                    tr.gameObject.GetComponent<Button>().onClick.AddListener(ExitZoom);
                    foreach (Transform uz in tr)
                        if (uz.name == "Unzoom")
                        {
                            uz.gameObject.SetActive(true);
                            uz.gameObject.GetComponent<Button>().onClick.AddListener(ExitZoom);
                        }
                }
                if (tr.name == "MainImage")
                    mainImage = tr.gameObject;
               
            }
            NewZoomedImage.GetComponent<Image>().enabled = false;
            ZoomImageNewParent.gameObject.SetActive(true);
            mainImage.transform.localScale = new Vector2(1f, 1f);
            isZoomed = true;
        }
        void ExitZoom()
        {
            //Tick = true;
            Destroy(NewZoomedImage);
            DisplayImage.SetActive(true);
            ZoomImageNewParent.gameObject.SetActive(false);
            isZoomed = false;
        }
        private void OnEnable()
        {
            GameOverComplete.SetActive(false);
            GameOverFailed.SetActive(false);
            PauseScreen.SetActive(false);
        }

        private void Start()
        {
            Localize();
            LocalizationManager.OnLanguageChanged += Localize;

            DOTween.Kill("pinScaler", true);
            pin.rectTransform.DOPunchScale(new Vector3(.5f, .5f, .5f), 0.5f, 0, 0).SetEase(Ease.Linear).SetDelay(0.5f).SetLoops(-1, LoopType.Restart).SetId("pinScaler");
        }

        public override void Open(params object[] args)
        {
            base.Open(args);

            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].GetType() == typeof(GameQuizData[]))
                    {
                        UpdateQuizInfo((GameQuizData[])args[i]);
                    }

                }
            }
        }

        public override void Close()
        {
            base.Close();
        }


        public override void CloseImmediate()
        {
            base.CloseImmediate();
        }
        void Localize()
        {
            CongratsTitle.text = LocalizationManager.localization.Congrats;

            CongratsTitle.GetComponent<ArabicText>().Text = CongratsTitle.text;
            CongratsTitle.GetComponent<ArabicText>().Refresh();

            NextButtonLabel.text = LocalizationManager.localization.Next;

            NextButtonLabel.GetComponent<ArabicText>().Text = NextButtonLabel.text;
            NextButtonLabel.GetComponent<ArabicText>().Refresh();

            reviewListHeader.text = LocalizationManager.localization.ReviewList;

            reviewListHeader.GetComponent<ArabicText>().Text = reviewListHeader.text;
            reviewListHeader.GetComponent<ArabicText>().Refresh();

            score.text = string.Format(LocalizationManager.isRightToLeftLanguage ? "{1} :{0}" : "{0}: {1}", LocalizationManager.localization.score, Score);
            GOscore.text = string.Format(LocalizationManager.isRightToLeftLanguage ? "{1} :{0}" : "{0}: {1}", LocalizationManager.localization.score, Score);
            bestScore.text = LocalizationManager.localization.bestScore;
            bigDeal.text = LocalizationManager.localization.bigDeal;
            pause.text = LocalizationManager.localization.pause;
            btn_ResumeQuiz.text = LocalizationManager.localization.btn_ResumeQuiz;
            btn_QuitQuiz.text = LocalizationManager.localization.btn_QuitQuiz;

            score.GetComponent<ArabicText>().Text = score.text;
            score.GetComponent<ArabicText>().Refresh();

            GOscore.GetComponent<ArabicText>().Text = GOscore.text;
            GOscore.GetComponent<ArabicText>().Refresh();

            bestScore.GetComponent<ArabicText>().Text = bestScore.text;
            bestScore.GetComponent<ArabicText>().Refresh();

            bigDeal.GetComponent<ArabicText>().Text = bigDeal.text;
            bigDeal.GetComponent<ArabicText>().Refresh();

            pause.GetComponent<ArabicText>().Text = pause.text;
            pause.GetComponent<ArabicText>().Refresh();

            btn_ResumeQuiz.GetComponent<ArabicText>().Text = btn_ResumeQuiz.text;
            btn_ResumeQuiz.GetComponent<ArabicText>().Refresh();

            btn_QuitQuiz.GetComponent<ArabicText>().Text = btn_QuitQuiz.text;
            btn_QuitQuiz.GetComponent<ArabicText>().Refresh();
        }

        void Update()
        {
            if (showAnswerButtons && Tick)
            {
                Timer -= Time.deltaTime;
                TimeText.text = timerEnabled ? Timer.ToString("00") : "";

                

                if (Timer <= 0 && timerEnabled)
                {
                    if(isZoomed) ExitZoom();
                    WrongAnswers.Add(AllQuestions[QuestionIndex].Question);
                    WrongCountText.text = WrongAnswers.Count.ToString();
                    AnsweredQuestion("false");
                    Timer = 20f;
                    TimeText.text = Timer.ToString("00");

                    
                }

                for (int i = 0; i < AnswerBtns.Length; i++)
                {
                    if (!AnswerBtns[i].activeSelf)
                    {
                        AnswerBtns[i].transform.localScale = Vector3.zero;
                        AnswerBtns[i].SetActive(true);
                        AnswerBtns[i].transform.DOScale(1f, 0.15f).SetDelay(0.1f * i);
                    }
                }
            }
            else if (!showAnswerButtons)
            {
                for (int i = 0; i < AnswerBtns.Length; i++)
                {
                    if (AnswerBtns[i].activeSelf) AnswerBtns[i].SetActive(false);
                }
            }


        }

        public void StartQuiz()
        {
            Screen_Home.showAdOnOpen = true;
            MusicManager.Instance.PlayMusic("quiz_music");
            ResetGame();
            Timer = 20.5f;
            Tick = true;
        }

        void ResetGame()
        {
            if (isZoomed) ExitZoom();
            QuestionIndex = 0;
            AnswerIndex = 0;
            WrongAnswers.Clear();
            CorrectAnswers.Clear();
            CorrectCountText.text = "0";
            WrongCountText.text = "0";
            Score = 0;
            score.text = string.Format(LocalizationManager.isRightToLeftLanguage ? "{1} :{0}" : "{0}: {1}", LocalizationManager.localization.score, Score);

            score.GetComponent<ArabicText>().Text = score.text;
            score.GetComponent<ArabicText>().Refresh();
            showAnswerButtons = false;
        }

        bool showAnswerButtons = false;
        bool canClickAnswer = false;

        public void AnsweredQuestion(string correct)
        {

            if (correct == "true")
            {
                if (timerEnabled)
                {
                    Score += (int)Timer;
                }
                else
                {
                    Score += 5;
                }
            }

            if (isZoomed) ExitZoom();

            Timer = 20.5f;
            Tick = true;
            score.text = LocalizationManager.localization.score + ": " + Score;
            score.GetComponent<ArabicText>().Text = score.text;
            score.GetComponent<ArabicText>().Refresh();
            if (QuestionIndex < AllQuestions.Length - 1)
            {
                QuestionIndex++;
                StartCoroutine(DelayNextQuestion(QuestionIndex));
            }
            else
            {
                StartCoroutine(GameOver());
            }
        }

        IEnumerator GameOver()
        {
            Tick = false;
            if (isZoomed) ExitZoom();
            yield return new WaitForSeconds(2f);

            MusicManager.Instance.PlayMusic("menu_music");

            ShowGameOverFailed();

        }

        [SerializeField]
        Button bragButton;

        [SerializeField]
        Text bragName, bragPercent, bragCategory;

        void ShowGameOverComplete()
        {
            CanvasGroup gr = GameOverComplete.GetComponent<CanvasGroup>();

            if (gr == null) gr = GameOverComplete.AddComponent<CanvasGroup>();

            gr.alpha = 0;

           // bragName.text = User.current.DisplayName;

            bragPercent.text = string.Format((((float)CorrectAnswers.Count / AllQuestions.Length) * 100).ToString("n0") + "% {0}!", LocalizationManager.localization.Correct);

            string categoryLocalized = AllQuestions[0].Category.Substring(5).Replace("Pathology", "");

            if (LocalizationManager.CurrentLanguage.tag != "en")
            {
                QuizCategory qc = GameManager.Instance.gameData.categories.Find(x => x.name.ToLower() == AllQuestions[0].Category.ToLower());
                categoryLocalized = qc.GetLocalizedName();
            }

            bragCategory.text = string.Format(LocalizationManager.isRightToLeftLanguage ? "{1} {0}" : "{0} {1}", categoryLocalized , LocalizationManager.localization.Quiz);
            bragCategory.GetComponent<ArabicText>().Text = bragCategory.text;
            bragCategory.GetComponent<ArabicText>().Refresh();


            GameOverFailed.SetActive(false);
            GameOverComplete.SetActive(true);
            gr.DOFade(1f, 0.25f).OnComplete(() =>
            {
                GetScreenshotReady();

                bragButton.onClick.RemoveAllListeners();
                bragButton.onClick.AddListener(() =>
                {
                    MusicManager.Instance.PlaySound("swipe");
                    MobileManager.ShareBragImage(screenshot);
                });

            });
        }


        Texture2D screenshot;
        [SerializeField]
        RectTransform ScreenshotCaptureArea;

        void GetScreenshotReady()
        {
            StartCoroutine(DoGetScreenshotReady());
        }

        //void OnGUI()
        //{
        //    Canvas canvas = FindObjectOfType<Canvas>();

        //    //GUILayout.Label("Canvas Height: " + canvas.pixelRect.height);
        //    //GUILayout.Label("Canvas Width: " + canvas.pixelRect.width);

        //    //GUILayout.Label("Screen Height: " + UnityEngine.Screen.height);
        //    //GUILayout.Label("Screen Width: " + UnityEngine.Screen.width);

        //    GUILayout.Label("Have Screenshot Ready?: " + (screenshot != null));

        //    Vector2 size = Vector2.Scale(ScreenshotCaptureArea.rect.size, ScreenshotCaptureArea.lossyScale);
        //    //float x = ScreenshotCaptureArea.position.x + ScreenshotCaptureArea.anchoredPosition.x;
        //    //float y = /*UnityEngine.Screen.height - */ScreenshotCaptureArea.position.y - ScreenshotCaptureArea.anchoredPosition.y;

        //    Vector2 screenPos = ScreenshotCaptureArea.position;
        //    float x = screenPos.x;// ScreenshotCaptureArea.anchoredPosition.x;
        //    float y = screenPos.y;



        //    Rect r = new Rect(x, y, size.x, size.y);
        //    //Debug.Log("Modified rect = " + r.ToString());



        //    GUI.skin.label.normal.textColor = Color.green;
        //    GUI.Label(new Rect(r.x, UnityEngine.Screen.height - r.y, 100, 30), "X-0");
        //    GUI.Label(new Rect(r.x + r.width, UnityEngine.Screen.height - (r.y + r.height), 200, 100), "Y-0");

        //}

        IEnumerator DoGetScreenshotReady()
        {
            yield return new WaitForEndOfFrame();
            //Vector2 size = Vector2.Scale(ScreenshotCaptureArea.rect.size, ScreenshotCaptureArea.lossyScale);
            //float x = ScreenshotCaptureArea.position.x + ScreenshotCaptureArea.anchoredPosition.x;
            //float y = UnityEngine.Screen.height - ScreenshotCaptureArea.position.y - ScreenshotCaptureArea.anchoredPosition.y;

            //Rect captureRect = new Rect(x, y, size.x, size.y);
            //============================================




            Vector2 size = Vector2.Scale(ScreenshotCaptureArea.rect.size, ScreenshotCaptureArea.lossyScale);
            //float x = ScreenshotCaptureArea.position.x + ScreenshotCaptureArea.anchoredPosition.x;
            //float y = /*UnityEngine.Screen.height - */ScreenshotCaptureArea.position.y - ScreenshotCaptureArea.anchoredPosition.y;

            Vector2 screenPos = ScreenshotCaptureArea.position;
            float x = screenPos.x;// ScreenshotCaptureArea.anchoredPosition.x;
            float y = screenPos.y;



            Rect captureRect = new Rect(x, y, size.x, size.y);

            screenshot = new Texture2D((int)captureRect.width, (int)captureRect.height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(captureRect, 0, 0);
            screenshot.Apply();
        }

        [SerializeField]
        Slider GOSlider;

        [SerializeField]
        Button gameOverNextButton;

        public void ShowGameOverFailed()
        {
            foreach (Transform tr in Content)
            {
                Destroy(tr.gameObject);
            }
            for (int i = 0; i < WrongAnswers.Count; i++)
            {
                GameObject GO = Instantiate(failedEntry, Content);
                GO.GetComponent<Text>().text = WrongAnswers[i].ClassificationText;

                GO.GetComponent<ArabicText>().Text = WrongAnswers[i].ClassificationText;
                GO.GetComponent<ArabicText>().Refresh();
            }

            StartCoroutine(DoShowGameOverFailed());
        }

        IEnumerator DoShowGameOverFailed()
        {
            GameOverComplete.SetActive(false);

            CanvasGroup gr = GameOverFailed.GetComponent<CanvasGroup>();

            if (gr == null) gr = GameOverFailed.AddComponent<CanvasGroup>();

            gr.alpha = 0;

            

            GOSlider.minValue = 0f;
            GOSlider.maxValue = 1f;
            GOSlider.value = 0f;
            
            GOscore.text = string.Format(LocalizationManager.isRightToLeftLanguage ? "{1} :{0}" : "{0}: {1}", LocalizationManager.localization.score, "0");
            bestScore.text = string.Format(LocalizationManager.isRightToLeftLanguage ? "{1} :{0}" : "{0}: {1}", LocalizationManager.localization.bestScore, BestScore.ToString("n0"));
            GOCorrectAnswers.text = string.Format(LocalizationManager.localization.AmountCorrect, CorrectAnswers.Count, AllQuestions.Length);

            GOscore.GetComponent<ArabicText>().Text = GOscore.text;
            GOscore.GetComponent<ArabicText>().Refresh();

            bestScore.GetComponent<ArabicText>().Text = bestScore.text;
            bestScore.GetComponent<ArabicText>().Refresh();

            GOCorrectAnswers.GetComponent<ArabicText>().Text = GOCorrectAnswers.text;
            GOCorrectAnswers.GetComponent<ArabicText>().Refresh();

            GameOverFailed.SetActive(true);

            yield return new WaitForSeconds(0.25f);

            float percentCorrect = (float)CorrectAnswers.Count / AllQuestions.Length;
            bestScore.color = Color.white;
            gr.DOFade(1f, 0.25f).OnComplete(() =>
            {
                MusicManager.Instance.PlaySound("quiz_scorebar");
                DOVirtual.Float(0f, 1f, 3f, (progress) =>
                {
                    GOSlider.value = percentCorrect * progress;
                    GOscore.text = string.Format(LocalizationManager.isRightToLeftLanguage ? "{1} :{0}" : "{0}: {1}", LocalizationManager.localization.score, (Score * progress).ToString("n0"));

                    GOscore.GetComponent<ArabicText>().Text = GOscore.text;
                    GOscore.GetComponent<ArabicText>().Refresh();

                }).OnComplete(() =>
                {
                    if (Score > BestScore)
                    {
                        BestScore = Score;
                        bestScore.text = string.Format(LocalizationManager.isRightToLeftLanguage ? "{1} :{0}" : "{0}: {1}", LocalizationManager.localization.bestScore, BestScore);
                        bestScore.DOColor(Color.green, 0.25f).SetLoops(1, LoopType.Yoyo);
                        bestScore.rectTransform.DOPunchScale(Vector3.one * .25f, 0.5f, 0, 0);

                        bestScore.GetComponent<ArabicText>().Text = bestScore.text;
                        bestScore.GetComponent<ArabicText>().Refresh();
                    }
                });

            });


            //Update the running tally on the leaderboards for this players total score of all the quizzes they've ever played.
            //   GSManager.SubmitScore(Score, GameManager.CategoryToLeaderboardID(AllQuestions[0].Category));

            FirebaseAuthenticationsHandler.instance.SaveDataButton(AllQuestions[0].Category, Score);
            UIManager.instance.leaderboardButton.gameObject.SetActive(true);
            if (PlayerPrefs.GetString("Showed Rate App", "false") == "false")
            {
                RateBox.Instance.Show("Enjoying Anatomist?", "Give us a 5 star rating to help us grow!", "Rate Us", "Later");
                PlayerPrefs.SetString("Showed Rate App", "true");
            }

            //if (FB.IsLoggedIn)
            //{
                gameOverNextButton.onClick.RemoveAllListeners();
                gameOverNextButton.onClick.AddListener(() => { ShowGameOverComplete(); });
          //  }
          //  else
          //  {
           //     gameOverNextButton.onClick.RemoveAllListeners();
           //     gameOverNextButton.onClick.AddListener(() => { Anatomist.Screen.Find("home").Open(); });
           // }
        }

        public void CorrectAnswer(Image img)
        {
            if (!canClickAnswer) return;

            canClickAnswer = false;
            Correct = img.gameObject;
            ChangeAllColorButtons(UnselectedColor);
            img.color = CorrectColor;
            PlayerPrefs.SetString(AllQuestions[QuestionIndex].Question.ClassificationText, "true");
            CorrectAnswers.Add(AllQuestions[QuestionIndex].Question);
            CorrectCountText.text = CorrectAnswers.Count.ToString();
            MusicManager.Instance.PlaySound("correct");
            AnsweredQuestion("true");
        }

        public void WrongAnswer(Image img)
        {
            if (!canClickAnswer) return;

            canClickAnswer = false;
            Wrong = img.gameObject;
            ChangeAllColorButtons(UnselectedColor);
            img.color = WrongColor;
            AnswerBtns[AnswerIndex].GetComponent<Image>().color = CorrectColor;
            WrongAnswers.Add(AllQuestions[QuestionIndex].Question);
            WrongCountText.text = WrongAnswers.Count.ToString();
            MusicManager.Instance.PlaySound("wrong");
            AnsweredQuestion("false");
        }

        public void UpdateQuizInfo(GameQuizData[] data)
        {
            if (data == null || data.Length == 0)
            {
                Debug.LogError("NO QUIZ DATA LOADED");
                return;
            }
            AllQuestions = data;
            StartQuiz();
            NextQuestion(0);
        }

        [SerializeField]
        Image pin;

        void NextQuestion(int index)
        {
            pin.gameObject.SetActive(false);
            StartCoroutine(GetNextQuestion(index));
        }

        IEnumerator GetNextQuestion(int questionIndex)
        {
            showAnswerButtons = false;
            yield return new WaitForEndOfFrame();
            ChangeAllColorButtons(DefaultColor);
            question.text = LocalizationManager.localization.question + " # \n " + (questionIndex + 1) + " of " + AllQuestions.Length;

            question.GetComponent<ArabicText>().Text = question.text;
            question.GetComponent<ArabicText>().Refresh();

            if (AllQuestions[questionIndex].Question.type == QuizType.Image)
            {
                QuestionText.gameObject.SetActive(false);
                QuestionText.transform.parent.gameObject.SetActive(false);

                pin.gameObject.SetActive(true);
                GameManager.Instance.AttachPinToImage(pin, QuestionImage, AllQuestions[questionIndex].Question);
                QuestionImage.enabled = false;
                Texture2D questionTexture = AllQuestions[questionIndex].Question.Texture;
                QuestionImage.sprite = Sprite.Create(questionTexture, new Rect(0, 0, questionTexture.width, questionTexture.height), new Vector2(0.5f, 0.5f));

                yield return new WaitForSeconds(0.15f);

                QuestionImage.enabled = true;

                QuestionImage.gameObject.SetActive(true);
                SourceText.text = "Source: " + GameManager.Instance.GetSource(AllQuestions[questionIndex].Question.QuestionText);
              
                Search.SetActive(true);
                CC.SetActive(true);

            }
            else
            {
                pin.gameObject.SetActive(false);
                QuestionText.text = AllQuestions[questionIndex].Question.QuestionText;

                QuestionText.GetComponent<ArabicText>().Text = QuestionText.text;
                QuestionText.GetComponent<ArabicText>().Refresh();

                QuestionText.transform.parent.gameObject.SetActive(true);
                QuestionText.gameObject.SetActive(true);
                QuestionImage.gameObject.SetActive(false);
                CC.SetActive(false);
                SourceText.text = "";
                Search.SetActive(false);
            }

            //Determine which of the 4 buttons will be the correct answer position
            AnswerIndex = Random.Range(0, AnswerBtns.Length);

            int curWrongAnswerIndex = 0;

            //Set up each of the answer buttons, and put the correct answer on the index that was chosen
            for (int i = 0; i < AnswerBtns.Length; i++)
            {
                bool isAnswer = false;
                string answerText = "";

                if (i != AnswerIndex)
                {
                    answerText = AllQuestions[questionIndex].WrongAnswers[curWrongAnswerIndex];
                    curWrongAnswerIndex++;
                }
                else
                {
                    isAnswer = true;
                    answerText = AllQuestions[questionIndex].Question.AnswerText;
                }
                if (!string.IsNullOrEmpty(answerText))
                {
                    //Debug.Log("Answer Created: " + (isAnswer == true ? "CORRECT ANSWER: " : "WRONG ANSWER: ") + answerText);
                    AnswerBtns[i].GetComponent<AnswerBtns>().UpdateAnswerBtn(isAnswer, answerText);
                }
            }

            if (isZoomed) ExitZoom();
            Debug.LogWarning("Question Index: " + questionIndex);
            Debug.LogWarning("Index modulus? " + (questionIndex + 1) % 5);
            if ((questionIndex + 1) % 5 == 0)
            {
                Debug.LogWarning("<color=Red>#######</color>   SHOWING AD");
                AdsManager.Instance.ShowInterstitialAD(() =>
                {
                    Debug.LogWarning("<color=Magenta>AD CLOSED EVENT FIRED</color>");
                    showAnswerButtons = true;
                    canClickAnswer = true;
                    Tick = true;
                });
            }
            else
            {
                //Question index is not a multiple of 5, so we just proceed without showing an ad.
                Debug.Log("NOT A MULTIPLE OF 5");
                showAnswerButtons = true;
                canClickAnswer = true;
                Tick = true;
            }
        }


        public void Pause()
        {
            if (isZoomed) ExitZoom();
            Tick = false;
            PauseScreen.SetActive(true);
        }

        public void UnPause()
        {
            StartCoroutine(DelayPause());
            PauseScreen.SetActive(false);
        }
        public IEnumerator DelayPause()
        {
            yield return new WaitForSeconds(0.5f);
            Tick = true;
        }
        public IEnumerator DelayNextQuestion(int index)
        {
            Tick = false;

            yield return new WaitForSeconds(1f);
            NextQuestion(index);
        }
        public void QuitQuiz()
        {
            if (isZoomed) ExitZoom();
            Anatomist.Screen.Find("Home").Open();
            Tick = false;
        }
        void ChangeAllColorButtons(Color col)
        {
            for (int i = 0; i < AnswerBtns.Length; i++)
            {
                AnswerBtns[i].GetComponent<Image>().color = col;
            }
        }
    }
}