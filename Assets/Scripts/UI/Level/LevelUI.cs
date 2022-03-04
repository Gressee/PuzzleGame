using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Core.Singleton;
using Shared.Defines;

public class LevelUI : Singleton<LevelUI>
{
    [SerializeField]
    Button btnExecute, btnRevert, btnPause;
    [SerializeField]
    Canvas canLvlSolevdOverlay;

    void Start()
    {
        // Add anonymus methods to the button on click events

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
    }

    public void SpawnLevelSolvedOverlay()
    {
        canLvlSolevdOverlay.gameObject.SetActive(true);
    }
}
