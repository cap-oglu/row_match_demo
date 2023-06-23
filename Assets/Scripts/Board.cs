using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour
{
    [HideInInspector]
    public int width;
    [HideInInspector]
    public int height;

    public GameObject bgTilePrefab;
    public Gem[] gems;
    public Gem[,] allGems;

    public float gemSpeed;

    [HideInInspector]
    public MatchFinder matchFind;

    public enum BoardState { wait, move };
    public BoardState currentState = BoardState.move;

    public RoundManager roundManager;

    //public GameObject levelData;

    public List<LevelData> levels;
    private string levelDataPath = "Assets/Resources/RM_A";

    string sceneName;
    int sceneNumber;

    private void Awake()
    {   
        //levelData = GameObject.FindWithTag("Level" + levels[i].levelNumber.ToString()));
        matchFind = FindObjectOfType<MatchFinder>();
        roundManager = FindObjectOfType<RoundManager>();
        
        //levelData = GameObject.FindWithTag("FileReader");
        /*if(levelData == null)
        {
            Debug.Log("LevelData is null");
        }*/
        LoadLevelData();
        sceneName = SceneManager.GetActiveScene().name;
        string numericPart = sceneName.TrimStart('L');
        sceneNumber = int.Parse(numericPart);
        width = levels[sceneNumber - 1].gridWidth;
        height = levels[sceneNumber - 1].gridHeight;
        roundManager.roundNumber = levels[sceneNumber - 1].moveCount;
       
        //width = 7;
        //height = 7;
    }
    // Start is called before the first frame update
    void Start()
    {   
        allGems = new Gem[width, height];
        //roundManager.roundNumber = 12;
        Setup();
        
    }

    // Update is called once per frame
    private void Setup()
    {   
        
        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                 Vector2 pos = new Vector2(j, i);
                 GameObject bgTile = Instantiate(bgTilePrefab, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
                 bgTile.transform.parent = transform;
                 bgTile.name = "BG Tile - " + j + ", " + i;

                int gemToUse = Random.Range(0, gems.Length);

                //int iterations = 0;
                /*while(MatchesAt(new Vector2Int(i, j), gems[gemToUse]) && iterations < 100)
                {   

                    gemToUse = Random.Range(0, gems.Length);
                    iterations++;   

                }*/

                if (levels[sceneNumber - 1].grid[j + i * width] == "b")
                {
                    gemToUse = 0;
                }
                else if (levels[sceneNumber - 1].grid[j + i * width] == "y") 
                {
                    gemToUse = 3;
                }
                else if (levels[sceneNumber - 1].grid[j + i * width] == "r")
                {
                    gemToUse = 2;
                }
                else if (levels[sceneNumber - 1].grid[j + i * width] == "g")
                {
                    gemToUse = 1;
                }
                

                SpawnGem(new Vector2Int(j, i), gems[gemToUse]);
            }
        }
    }

    private void Update()
    {
        //matchFind.FindAllMatches();
    }

    private void SpawnGem(Vector2Int pos, Gem gemToSpawn)
    {
        Gem gem = Instantiate(gemToSpawn, new Vector3(pos.x, pos.y + height, 0f), Quaternion.identity);
        gem.transform.parent = this.transform;
        gem.name = "Gem - " + pos.x + ", " + pos.y;
        allGems[pos.x,pos.y] = gem;
        gem.SetupGem(pos, this);
    }

    /*bool MatchesAt(Vector2Int posToCheck, Gem gemToCheck)
    {

        if(posToCheck.x > 1)
        {
            if (allGems[posToCheck.x - 1, posToCheck.y].type == gemToCheck.type && allGems[posToCheck.x - 2, posToCheck.y].type == gemToCheck.type)
            {
                return true;
            }
        }

        if (posToCheck.y > 1)
        {
            if (allGems[posToCheck.x, posToCheck.y - 1].type == gemToCheck.type && allGems[posToCheck.x, posToCheck.y - 2].type == gemToCheck.type)
            {
                return true;
            }
        }


        return false;
    }*/

    public void DestroyMatchedGemAt(Vector2Int pos)
    {
        if (allGems[pos.x, pos.y] != null)
        {
            if (allGems[pos.x, pos.y].isMatched)
            {   
                Instantiate(allGems[pos.x, pos.y].destroyEffect, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
                Destroy(allGems[pos.x, pos.y].gameObject);
                allGems[pos.x, pos.y] = null;
            }
        }

        
    }

    public void DestroyMatches()
    {
        for(int i = 0; i < matchFind.currentMatches.Count; i++)
        {
            if (matchFind.currentMatches[i] != null)
            {
                ScoreCheck(matchFind.currentMatches[i]); //gem type'a göre ayarla 
                DestroyMatchedGemAt(matchFind.currentMatches[i].posIndex);
            }
        }
        
        //round number azalt
        roundManager.roundNumber--;

        //StartCoroutine(DecreaseRowCo());
        StartCoroutine(FillBoardCo());
        currentState = BoardState.move;
    }

    private IEnumerator DecreaseRowCo()
    {
        yield return new WaitForSeconds(.2f);

        int nullCounter = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i,j] == null)
                {
                        nullCounter++;
                }
                else if(nullCounter > 0)
                {
                    allGems[i, j].posIndex.y -= nullCounter;
                    allGems[i, j - nullCounter] = allGems[i, j];
                    allGems[i, j] = null;
                }
            }
            nullCounter = 0;
        }
    }

    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(.5f);

        RefillBoard();
        /*
        yield return new WaitForSeconds(.5f);
        
        matchFind.FindAllMatches();

        if (matchFind.currentMatches.Count > 0)
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches(); //recursion
        }
        else { 
            yield return new WaitForSeconds(.5f);
            currentState = BoardState.move;
            //round number azalt
            roundManager.roundNumber--;
        }
        */
    }

    private void RefillBoard()
    {   /*
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i,j] == null)
                {
                    int gemToUse = Random.Range(0, gems.Length);
                    SpawnGem(new Vector2Int(i, j), gems[gemToUse]);
                }
                 
            }
        }
        */
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] == null)
                {
                    int gemToUse = 4;
                    SpawnGem(new Vector2Int(i, j), gems[gemToUse]);
                }

            }
        }
    }

    private void CheckMisplacedGems() 
    {
        List<Gem> foundGems = new List<Gem>();
        foundGems.AddRange(FindObjectsOfType<Gem>());


        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (foundGems.Contains(allGems[i,j]))
                {
                    foundGems.Remove(allGems[i, j]);
                }
            }
        }

        foreach(Gem gem in foundGems)
        {
           Destroy(gem.gameObject);
        }
    }

    /*public void ShuffleBoard()
    {

    }*/

    public void ScoreCheck(Gem gemToCheck)
    {
        roundManager.currentScore += gemToCheck.gemPoint[(int)gemToCheck.type];
    }

    void LoadLevelData()
    {
        levels = new List<LevelData>();

        for (int i = 0; i < 25; i++)
        {

            levelDataPath = "Assets/Resources/RM_A" + (i + 1).ToString();

            //string[] levelDataLines = File.ReadAllLines(levelDataPath);
            string[] levelDataLines;
            try
            {
                levelDataLines = File.ReadAllLines(levelDataPath);
            }
            catch
            {
                continue;
            }

            LevelData level = new LevelData();

            for (int j = 0; j < levelDataLines.Length; j++)
            {
                string line = levelDataLines[j];

                ParseLevelData(line, ref level);


            }

            levels.Add(level);

        }
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


}
