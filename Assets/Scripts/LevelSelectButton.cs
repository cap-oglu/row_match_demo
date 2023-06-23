using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{

    public string levelToLoad;
    // Start is called before the first frame update
    void Awake()
    {   

       
    }
    void Start()
    {
        
    }
        // Update is called once per frame
        void Update()
        {
        //Debug.Log(PlayerPrefs.GetInt(levelToLoad));
        if (PlayerPrefs.GetInt(levelToLoad) == 1)
        {
            GetComponent<UnityEngine.UI.Button>().interactable = true;
            this.GetComponent<UnityEngine.UI.Image>().color = Color.green;


        }
        else
        {
            this.GetComponent<UnityEngine.UI.Button>().interactable = false;
            this.GetComponent<UnityEngine.UI.Image>().color = Color.gray;
        }
    }

         public void LoadLevel()
        {
            if (PlayerPrefs.GetInt(levelToLoad) == 1)
            {
                SceneManager.LoadScene(levelToLoad);
            }

        }
    
}
