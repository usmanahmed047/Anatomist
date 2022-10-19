using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Org.BouncyCastle.Asn1.X509;
using Time = UnityEngine.Time;

public static class AnatomistExtensions
{
    public static void Reset(this Transform t)
    {
        t.localScale = Vector3.one;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
    }
}
namespace Anatomist
{
    public class Screen_Search : Screen
    {

        public int itemsPerPage = 50;

        public static Screen_Search Instance;
        [SerializeField]
        Transform Content;
        [SerializeField]
        GameObject EntryObject;
        [SerializeField]
        InputField SearchString;

        
        public GameObject SearchDisplay;

        [SerializeField]
        Text searchTop, searchFieldEmpty;

        List<SearchEntry> searchEntries = new List<SearchEntry>();

        void Awake()
        {
            Instance = this;
           

        }

        private void Start()
        {
            Localize();
            LocalizationManager.OnLanguageChanged += Localize;
        }

        void OnEnable()
        {
            SearchString.text = "";
            Invoke("Search", 0.5f);
        }

        float lastSearchChangeTime = 0f;
        public void Search()
        {
            lastSearchChangeTime = Time.time;
            if(SearchString.text.Length >= 3 || string.IsNullOrEmpty(SearchString.text))
                LoadResults(SearchString.text, 0, itemsPerPage);
        }


        [SerializeField]
        int lastSearchResultIndex = 0;
        List<QuizQuestion> searchData = new List<QuizQuestion>();
        bool searching = false;
        string lastSearchString = "-1";

        public void LoadResults(string searchString, int startingIndex, int count)
        {
            if (searching) return;
            searching = true;


            if (lastSearchString != searchString)
            {
                results = new List<QuizQuestion>();
                lastSearchString = searchString;
                lastSearchResultIndex = 0;

                if (string.IsNullOrEmpty(searchString))
                {
                    Debug.Log("Showing ALL search results because search is empty");
                    Debug.Log("searchData.Count = " + searchData.Count);
                    results = searchData;
                }
                else
                {
                    //Valid search string, find specific results.. 

                    searchString = searchString.Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "").Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "");
                    string[] searchTerms = searchString.Split(new char[] { ' ', '_', ',', '-', '"' }, System.StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < searchTerms.Length; i++)
                    {
                        string item = searchTerms[i];
                        List<QuizQuestion> set = searchData.FindAll(x => x.ClassificationText.ToUpper().Contains(item.ToUpper()) || x.QuestionText.ToUpper().Contains(item.ToUpper()));
                        if (set != null && set.Count > 0)
                            results.AddRange(set);
                    }
                }

            }
            else
            {
                lastSearchResultIndex = startingIndex;
            }

            SpawnResults();

            
            searching = false;
        }

        List<QuizQuestion> results = new List<QuizQuestion>();

        void SpawnResults()
        {
            searchEntries.Clear();
            ClearChild(Content);

            int index = lastSearchResultIndex;

            if (index > 0)
            {
                GameObject more = Instantiate(EntryObject, Content);
                more.GetComponentInChildren<TextMeshProUGUI>().text = "Go Back";
                more.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
                Destroy(more.GetComponent<SearchEntry>());
                more.GetComponent<Button>().onClick.RemoveAllListeners();

                int prevIndex = index - itemsPerPage;
                more.GetComponent<Button>().onClick.AddListener(() =>
                {
                    
                    this.LoadResults(lastSearchString, Mathf.Clamp(prevIndex, 0, results.Count-1), itemsPerPage);
                   // Content.Reset();
                });
            }


            for (int i = 0; i < itemsPerPage; i++)
            {
                Debug.Log("SPawning one: " + index + " vs " + results.Count);
                if (index < results.Count)
                {
                    QuizQuestion q = results[index];
                    GameObject entry = Instantiate(EntryObject, Content);
                    entry.GetComponentInChildren<Text>().text = q.ClassificationText;
                    entry.GetComponent<SearchEntry>().QuestionData = q;

                    entry.GetComponent<ArabicText>().enabled = false;
                    if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
                    {
                        entry.GetComponent<ArabicText>().enabled = true;
                        entry.GetComponentInChildren<Text>().GetComponent<ArabicText>().Text = q.ClassificationText;
                        entry.GetComponentInChildren<Text>().GetComponent<ArabicText>().Refresh();
                    }

                    searchEntries.Add(entry.GetComponent<SearchEntry>());
                    index++;
                }

            }

            lastSearchResultIndex = index;

            if (index < results.Count)
            {
                GameObject more = Instantiate(EntryObject, Content);
                more.GetComponentInChildren<Text>().text = "Show More";
              //  more.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
                Destroy(more.GetComponent<SearchEntry>());
                more.GetComponent<Button>().onClick.RemoveAllListeners();
                more.GetComponent<Button>().onClick.AddListener(() =>
                {
                    this.LoadResults(lastSearchString, lastSearchResultIndex, itemsPerPage);
                    Content.Reset();
                });
            }
        }
        
        public void BuildSearchResults()
        {
            Debug.Log("Building Search Results List!!!");
            
            for(int a = 0; a < GameManager.Instance.gameData.categories.Count; a++)
            {
                QuizCategory cat = GameManager.Instance.gameData.categories[a];

                if (cat.name.ToUpper().Contains("MEDICINE")) continue;


                for (int i = 0; i < cat.questions.Count; i++)
                {
                    QuizQuestion q = cat.questions[i];
                    searchData.Add(q);
                }
            }

            searchData = searchData.OrderBy(x => x.ClassificationText).ToList();
        }

        public SearchEntry GetNextSearchEntry(SearchEntry currentEntry)
        {
            int index = searchEntries.IndexOf(currentEntry);
            ++index;

            if (index >= searchEntries.Count)
                index = 0;

            return searchEntries[index];

        }

        public SearchEntry GetPrevSearchEntry(SearchEntry currentEntry)
        {
            int index = searchEntries.IndexOf(currentEntry);
            --index;

            if (index < 0)
                index = 0;

            return searchEntries[index];

        }

        void ClearChild(Transform tr)
        {
            foreach (Transform TR in tr)
            {
                //  TR.gameObject.SetActive(false);
                Destroy(TR.gameObject);
            }
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
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
            searchTop.text = LocalizationManager.localization.searchTop;
            searchFieldEmpty.text = LocalizationManager.localization.searchFieldEmpty;

            searchTop.GetComponent<ArabicText>().enabled = false;
            if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
            {
                searchTop.GetComponent<ArabicText>().enabled = true;
                searchTop.GetComponent<ArabicText>().Text = searchTop.text;
                searchTop.GetComponent<ArabicText>().Refresh();
            }

            searchFieldEmpty.GetComponent<ArabicText>().enabled = false;
            {
                if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
                {
                    searchFieldEmpty.GetComponent<ArabicText>().enabled = true;
                    searchFieldEmpty.GetComponent<ArabicText>().Text = searchFieldEmpty.text;
                    searchFieldEmpty.GetComponent<ArabicText>().Refresh();
                }
                
            }
        }
    }
}