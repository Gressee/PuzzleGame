using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Core.Singleton;
using Shared.Defines;

public class LevelUI : Singleton<LevelUI>
{
    [SerializeField]
    Button btnExecute, btnRevert, btnPause;
    
    // Level Solved Overlay
    [SerializeField]
    Button btnMenu, btnReplay, btnNextLevel;


    [SerializeField]
    Canvas canLvlSolevdOverlay;

    void Start()
    {
        //// Add anonymus methods to the button on click events ////

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
        btnMenu.onClick.AddListener(() => {
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

    public void SpawnLevelSolvedOverlay()
    {
        canLvlSolevdOverlay.gameObject.SetActive(true);
    }
}
