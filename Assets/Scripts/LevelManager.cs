using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class LevelManager : MonoBehaviour
{

    [HideInInspector]
    public string levelToLoad;

    public TMP_Text totalMovesText;
    public TMP_Text highScoreText;
    public TMP_Text levelNumberText;
    
    



    void Awake()
    {
        //levels = FindObjectOfType<LevelConfigurator>().levels;
    }
    void Start()
    {

        
        
        
        
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    
}