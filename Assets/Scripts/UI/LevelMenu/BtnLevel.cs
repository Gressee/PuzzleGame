using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Core.Singleton;

public class BtnLevel : MonoBehaviour
{
    [SerializeField]
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

        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = levelNum.ToString();
        gameObject.name = $"BtnLevelMenu_{levelNum}";
    }
}
