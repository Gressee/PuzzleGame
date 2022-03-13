using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Core.Singleton;

public class LevelMenuBtn : MonoBehaviour
{
    int levelNum;

    void Start()
    {
        // Add the on click event in the button
        gameObject.GetComponent<Button>().onClick.AddListener(() => {
            string sceneName = levelNum.ToString();
            while (sceneName.Length < 2)
            {
                sceneName = "0" + sceneName;
            }

            sceneName = "Level_" + sceneName;

            Debug.Log($"Change Scene to \"{sceneName}\"");
            SceneTransManager.Instance.LoadScene(sceneName);
        });
    }


    public void Init(int lvlNum)
    {
        levelNum = lvlNum;
        gameObject.name = $"LevelMenuBtn_{levelNum}";
        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = levelNum.ToString();
    }

}
