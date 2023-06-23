using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    public int roundNumber;
    private UIManager uiManager;
    public bool endingRound = false;

    private Board board;     

    public int currentScore;

    public int highScore;

    public string levelToLoad = "Level Select";

    private void Awake()
    {
        //uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        uiManager = FindObjectOfType<UIManager>();
        board = FindObjectOfType<Board>();
    }
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
            
            if(roundNumber <= 0)
            {
                roundNumber = 0;

                endingRound = true;
                
            }

      if (board.currentState == Board.BoardState.move) {    
            MoveCheck();

        }

        if (endingRound && board.currentState == Board.BoardState.move)
        {
            WinCheck();
            endingRound = false;
        }
        uiManager.highScoreText.text = roundNumber.ToString();
        uiManager.movesText.text = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "_Highest Score Value").ToString();
        uiManager.scoreText.text = currentScore.ToString();
    }

    private void WinCheck()
    {
        board.currentState = Board.BoardState.wait; //oynatamamak için ekledim, tararken halen daha move edebiliyorum

        if (PlayerPrefs.HasKey(SceneManager.GetActiveScene().name + "_Highest Score Value")) 
        {      
            highScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "_Highest Score Value");
        }
        else
        {
            highScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "_Highest Score Value", 0);
        }

        if (currentScore >= highScore)
        {   

            uiManager.gameOverPanel.SetActive(true);
            uiManager.winScore.text = currentScore.ToString();

            highScore = currentScore;
            uiManager.winText.text = "Congratulations!";
            uiManager.winStars.SetActive(true);
            //uiManager.winStars.GetComponent<RectTransform>().localScale = 2;
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Highest Score Value", currentScore);
            StartCoroutine(WaitForSceneLoad());
        }
        else {
            
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Highest Score Value", highScore);
            //Debug.Log(SceneManager.GetActiveScene().name);
            StartCoroutine(WaitForSceneLoad());
        }
     
    }

    private IEnumerator WaitForSceneLoad()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(levelToLoad);

    }

    private void MoveCheck()
    {
        int blueCounter = 0;
        int greenCounter = 0;
        int redCounter = 0;
        int yellowCounter = 0;
        for (int i = 0; i < board.height * board.width; i++)
        {
            
            if (board.allGems[i % board.width, i / board.width] != null) { 
                if(Gem.GemType.blue == board.allGems[i % board.width, i / board.width].type)
                {
                    blueCounter++;
                }
                else if (Gem.GemType.green == board.allGems[i % board.width, i / board.width].type)
                {
                    greenCounter++;
                }
                else if (Gem.GemType.red == board.allGems[i % board.width, i / board.width].type)
                {
                    redCounter++;
                }
                else if (Gem.GemType.yellow == board.allGems[i % board.width, i / board.width].type)
                {
                    yellowCounter++;
                }
                else if(Gem.GemType.stone == board.allGems[i % board.width, i / board.width].type &&
                    blueCounter < board.width && greenCounter < board.width && redCounter < board.width && yellowCounter < board.width)
                {
                    blueCounter = 0;
                    greenCounter = 0;
                    redCounter = 0;
                    yellowCounter = 0;
                }

            }

        }

        //Debug.Log("Blue: " + blueCounter + " Green: " + greenCounter + " Red: " + redCounter + " Yellow: " + yellowCounter);

        if(blueCounter < board.width && greenCounter < board.width && redCounter < board.width && yellowCounter < board.width)
        {
           WinCheck();
            
        }
    }
}
