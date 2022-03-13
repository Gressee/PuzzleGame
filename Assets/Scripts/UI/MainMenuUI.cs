using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Core.Singleton;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    Button btnPlay;

    void Start()
    {
        btnPlay.onClick.AddListener(() => {
            SceneTransManager.Instance.LoadScene("LevelMenu");
        });
    }
    
}
