using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Core.Singleton;
using Shared.Defines;

public class BtnNextLevel : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => {
            // Debug.LogWarning("Next Level currently unsupported");
            string currentScene = SceneManager.GetActiveScene().name;
            string[] array = currentScene.Split('_');
            string lvlNumString = array[array.Length -1];
            string lvlPack = array[array.Length - 2];

            int lvlNum;
            bool isParsable = int.TryParse(lvlNumString, out lvlNum);
            if (isParsable)
            {
                lvlNumString = (lvlNum+1).ToString();
                while (lvlNumString.Length < 2)
                {
                    lvlNumString = "0" + lvlNumString;
                }
                
                string newSceneName = $"Level_{lvlPack}_{lvlNumString}";
                
                if (!SceneTransManager.Instance.LoadScene(newSceneName))
                {
                    // If the scene can't be loaded because is does not exist
                    // (this happens if the currrent level was the last in the level Pack)
                    // go back to the level Menu
                    SceneTransManager.Instance.LoadScene("LevelMenu");    
                }
            }
            else
            {
                Debug.LogWarning("The name of the next scene couldn't be created");
                SceneTransManager.Instance.LoadScene("LevelMenu");
            }

        });
    }
}
