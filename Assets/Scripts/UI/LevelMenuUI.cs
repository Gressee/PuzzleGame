using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenuUI : MonoBehaviour
{
    [SerializeField]
    LevelMenuBtn lvlMenuBtnPrefab;

    const int totalLevels = 20;
    const int btnsPerRow = 5;

    void Start()
    { 
        // calc dist between buttons
        float canWidth = gameObject.GetComponent<Canvas>().GetComponent<RectTransform>().rect.width;
        float canHeight = gameObject.GetComponent<Canvas>().GetComponent<RectTransform>().rect.height;
        float btnDist = canWidth/ (btnsPerRow+1);

        // Spawn all the buttons
        for (int i = 1; i <= totalLevels; i++)
        {
            float x = ((i-1) % btnsPerRow + 1) * btnDist - canWidth/2;
            float y = -(Mathf.FloorToInt((i-1)/btnsPerRow) + 1) * btnDist + canHeight/2;

            LevelMenuBtn btn = Instantiate(lvlMenuBtnPrefab, Vector3.zero, Quaternion.identity);
            btn.transform.SetParent(gameObject.transform);
            btn.transform.localEulerAngles = Vector3.zero;
            btn.transform.localScale = Vector3.one;
            btn.transform.position = Vector3.zero;
            btn.transform.localPosition = new Vector3(x, y, 0);

            btn.Init(i);
        }
    }
}