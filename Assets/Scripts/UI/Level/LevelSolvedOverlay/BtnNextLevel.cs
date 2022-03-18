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
            Debug.LogWarning("Next Level currently unsupported");
        });
    }
}
