using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Core.Singleton;
using Shared.Defines;

public class LevelUI : Singleton<LevelUI>
{   
    // General
    [SerializeField]
    Button btnMainMenu;

    // Game Controll buttons
    [SerializeField]
    Button btnExecute, btnRevert, btnPause;
    
    // Level Solved Overlay
    [SerializeField]
    Button btnMenuInLSOverlay, btnReplay, btnNextLevel;


    [SerializeField]
    Canvas canLvlSolevdOverlay;

    GameState prevGameState = GameState.None;

    void Start()
    {
        //// Add anonymus methods to the button on click events ////

        // General Buttons
        btnMainMenu.onClick.AddListener(() => {
            SceneTransManager.Instance.LoadScene("MainMenu");
        });
        
        // Game Crontroll Button

        // Execute
        btnExecute.onClick.AddListener(() =>{
            GameManager.Instance.SetGameState(GameState.Execute);
        });

        // Pause
        btnPause.onClick.AddListener(() =>{
            GameManager.Instance.SetGameState(GameState.ExecutePause);
        });

        // Revert
        btnRevert.onClick.AddListener(() =>{
            GameManager.Instance.SetGameState(GameState.Building);
        });

        // Level Solved Overlay

        // Menu
        btnMenuInLSOverlay.onClick.AddListener(() => {
            SceneTransManager.Instance.LoadScene("LevelMenu");
        });

        // Replay
        btnReplay.onClick.AddListener(() => {
            SceneTransManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
        });

        // NextLevel
        btnNextLevel.onClick.AddListener(() => {
            // TODO make next level button function
            Debug.LogWarning("Next Level currently unsupported");
        });
    }

    void Update()
    {
        if (prevGameState != GameManager.Instance.CurrentGameState)
        {
            prevGameState = GameManager.Instance.CurrentGameState;
            UpdateControllButtons();
        }
    }
    
    void UpdateControllButtons() 
    {
        float CanvWidth = gameObject.GetComponent<RectTransform>().rect.width;
        float padding = 60; // Also applies to the y of the button
        float height = 200; // Height of the button

        // Chnage if the Revert, Pause, Execute Buttons show and how big they are
        switch (GameManager.Instance.CurrentGameState)
        {
            case GameState.Building:
                btnRevert.gameObject.SetActive(false);
                btnPause.gameObject.SetActive(false);
                btnExecute.gameObject.SetActive(true);

                btnExecute.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    CanvWidth-2*padding,
                    height
                );
                btnExecute.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    0,
                    padding
                );
            break;

            case GameState.ExecutePause:
                btnRevert.gameObject.SetActive(true);
                btnPause.gameObject.SetActive(false);
                btnExecute.gameObject.SetActive(true);
                
                btnRevert.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    CanvWidth/2.0f - 1.5f*padding,
                    height
                );
                btnRevert.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    -CanvWidth/4.0f,
                    padding
                );

                btnExecute.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    CanvWidth/2.0f - 1.5f*padding,
                    height
                );
                btnExecute.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    CanvWidth/4.0f,
                    padding
                );
            break;

            case GameState.Execute:
                btnRevert.gameObject.SetActive(true);
                btnPause.gameObject.SetActive(true);
                btnExecute.gameObject.SetActive(false);

                btnRevert.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    CanvWidth/2.0f - 1.5f*padding,
                    height
                );
                btnRevert.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    -CanvWidth/4.0f,
                    padding
                );

                btnPause.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    CanvWidth/2.0f - 1.5f*padding,
                    height
                );
                btnPause.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    CanvWidth/4.0f,
                    padding
                );
            break;
        }
    }

    public void SpawnLevelSolvedOverlay()
    {
        canLvlSolevdOverlay.gameObject.SetActive(true);
    }
}
