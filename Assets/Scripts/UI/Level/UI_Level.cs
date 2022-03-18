using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Core.Singleton;
using Shared.Defines;

public class UI_Level : Singleton<UI_Level>
{   
    // Game Controll buttons
    [SerializeField]
    Button btnExecute, btnRevert, btnPause;
    
    [SerializeField]
    Canvas canLvlSolevdOverlay;

    GameState prevGameState = GameState.None;


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
