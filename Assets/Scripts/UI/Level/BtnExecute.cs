using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core.Singleton;
using Shared.Defines;


public class BtnExecute : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => {
            GameManager.Instance.SetGameState(GameState.Execute);
        });    
    }
}
