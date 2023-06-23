using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLockUnlock : MonoBehaviour
{

    public GameObject level;
    // Start is called before the first frame update
    private void Awake()
    {
        
        PlayerPrefs.SetInt("L1", 1); // Locked
        for(int i = 2; i <= 25; i++)
        {
            string levelKey = "L" + i.ToString();
            PlayerPrefs.SetInt(levelKey, 0); // Locked
        }


    }
    void Start()
    {
        for (int i = 1; i <= 25; i++)
        {
            string levelKey = "L" + (i + 1).ToString();
            level = GameObject.FindWithTag("Level" + i.ToString());

            if (int.Parse(level.GetComponent<LevelManager>().highScoreText.text) > 0)
            {
                Debug.Log(level.GetComponent<LevelManager>().highScoreText.text);
                if (i < 25)
                {
                    UnlockNextLevel(levelKey);
                }

            }

        }




    }

    // Update is called once per frame
    void Update()
    {
       

    }

    public void UnlockNextLevel(string levelKey)
    {
        // Calculate the next level's key
       

        // Set the next level as unlocked
        PlayerPrefs.SetInt(levelKey, 1); // Unlocked
    }
}
