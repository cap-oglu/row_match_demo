using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{   
    public TMP_Text scoreText;
    public TMP_Text movesText;
    public TMP_Text highScoreText;

    public GameObject gameOverPanel;

    public TMP_Text winScore;
    public TMP_Text winText;
    public GameObject winStars;

    // Start is called before the first frame update
    void Start()
    {
        winStars.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
