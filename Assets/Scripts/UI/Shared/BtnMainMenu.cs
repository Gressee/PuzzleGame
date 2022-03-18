using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core.Singleton;

public class BtnMainMenu : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => {
            SceneTransManager.Instance.LoadScene("MainMenu");
        });
    }
}
