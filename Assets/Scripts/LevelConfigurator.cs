using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Threading.Tasks;

//10 tane dosyayý da oku
public class LevelConfigurator : MonoBehaviour
{

    public List<LevelData> levels;
    private const string levelDataKey = "LevelData";
    private string levelDataPath = "Assets/Resources/RM_A"; //sonra 1 kýsmýný deðþtir index ile
    public List<GameObject> levelList;



    //data'yý internetten indirmek için
    private string levelDataURL = "https://row-match.s3.amazonaws.com/levels/RM_A"; //A11'dan baþlýyor
    private string levelDataURL2 = "https://row-match.s3.amazonaws.com/levels/RM_B"; //B1'den baþlýyor
    private string savePath = "Assets/Resources/RM_A"; //A11'den baþlayacak
    //private string downloadFlagKey = "LevelDataDownloaded";


    private void Awake()
    {   
        
        // Load or create level data
        LoadLevelData();
        levelList = new List<GameObject>();
        for(int i = 0; i < levels.Count; i++)
        {
            if(GameObject.FindWithTag("Level" + levels[i].levelNumber.ToString()) != null) {
                levelList.Add(GameObject.FindWithTag("Level" + levels[i].levelNumber.ToString()));
            }
            
        }
        for(int i = 0; i < levelList.Count; i++)
        {
            levelList[i].GetComponent<LevelManager>().levelToLoad = "L" + (i + 1).ToString();
            levelList[i].GetComponent<LevelManager>().totalMovesText.text = levels[i].moveCount.ToString();
            levelList[i].GetComponent<LevelManager>().levelNumberText.text = "Level " + (i + 1).ToString();
            levelList[i].GetComponent<LevelManager>().highScoreText.text = PlayerPrefs.GetInt(levelList[i].GetComponent<LevelManager>().levelToLoad + "_Highest Score Value").ToString();
        }   

    }
    // Start is called before the first frame update
    void Start()
    {  
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async void LoadLevelData()
    {   

        levels = new List<LevelData>();

        

        for (int i = 0; i < 25; i++) { 

        levelDataPath = "Assets/Resources/RM_A" + (i + 1).ToString();
            string[] levelDataLines;
            try 
            {
                levelDataLines= File.ReadAllLines(levelDataPath);
                LevelData level = new LevelData();

                for (int j = 0; j < levelDataLines.Length; j++)
                {
                    string line = levelDataLines[j];

                    ParseLevelData(line, ref level);


                }

                levels.Add(level);
            }
            catch
            {
                 continue;
            }

            //string[] levelDataLines = File.ReadAllLines(levelDataPath);

            
           
        }
       

       await DownloadLevelData(11,25); //burada kaldýk.
    }

    private LevelData ParseLevelData(string line, ref LevelData level)
    {
        //LevelData level = new LevelData();

        string[] tokens = line.Split(':');
        if (tokens.Length == 2)
        {
            string key = tokens[0].Trim();
            string value = tokens[1].Trim();

            switch (key)
            {
                case "level_number":
                    level.levelNumber = int.Parse(value);
                    break;
                case "grid_width":
                    level.gridWidth = int.Parse(value);
                    break;
                case "grid_height":
                    level.gridHeight = int.Parse(value);
                    break;
                case "move_count":
                    level.moveCount = int.Parse(value);
                    break;
                case "grid":

                    level.grid = new List<string>(value.Split(','));
                    break;
                default:
                    Debug.LogWarning("Unknown level data key: " + key);
                    break;
            }
        }

        return level;
    }

    private async Task DownloadLevelData(int startLevel, int endLevel)
    {

        for (int i = startLevel; i <= endLevel; i++)
        {
            if (PlayerPrefs.GetInt("LevelDownload" + i.ToString()) == 1)
            {
                continue;
            }


            string levelURL = levelDataURL + i.ToString();
            if(i > 15)
            {
                levelURL = levelDataURL2 + (i - 15).ToString();
            }

            UnityWebRequest webRequest = UnityWebRequest.Get(levelURL);
            var operation = webRequest.SendWebRequest();

            // Create a TaskCompletionSource to await the request completion
            var tcs = new TaskCompletionSource<bool>();

            // Register a callback to set the TaskCompletionSource result when the request is completed
            operation.completed += (operation) =>
            {
                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Failed to download level data. Error: " + webRequest.error);
                    tcs.TrySetException(new System.Exception("Failed to download level data"));
                }
                else
                {
                    string levelData = webRequest.downloadHandler.text;
                    PlayerPrefs.SetInt("LevelDownload" + i.ToString(), 1);
                    SaveLevelData(levelData, i);
                    tcs.TrySetResult(true);
                }
            };

            // Wait for the TaskCompletionSource task to complete
            await tcs.Task;
            Debug.Log("LevelDownload" + i.ToString() + ": " + PlayerPrefs.GetInt("LevelDownload" + i.ToString()));
        }
            /*for(int i = 11; i <= 25; i++) {
           if(PlayerPrefs.GetInt("LevelDownload" + i.ToString()) == 1)
           {
               continue;
           }
           using (UnityWebRequest webRequest = UnityWebRequest.Get(levelDataURL + i.ToString()))
           {
               await Task.Yield();
               await webRequest.SendWebRequest();

               if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
               {
                   Debug.LogError("Failed to download level data. Error: " + webRequest.error);
               }
               else
               {   
                   string levelData = webRequest.downloadHandler.text;
                   PlayerPrefs.SetInt("LevelDownload" + i.ToString(), 1);
                   SaveLevelData(levelData, i);
               }
           }
        }*/
    }


    private void SaveLevelData(string levelData, int index)
    {
        System.IO.File.WriteAllText(savePath + index.ToString(), levelData);
        Debug.Log("Level data saved to: " + savePath + index.ToString());
    }
}

    



[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public int gridWidth;
    public int gridHeight;
    public int moveCount;
    public List<string> grid;
    // Add additional properties for level configuration
}


