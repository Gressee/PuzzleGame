using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField]
    GameObject GameManager, LevelCamera, LevelUI;

    void Start()
    {
        // Spawn all the GameObject that a Level needs all the Time
        Instantiate(LevelCamera, Vector3.zero, Quaternion.identity);
        Instantiate(GameManager, Vector3.zero, Quaternion.identity);
        Instantiate(LevelUI, Vector3.zero, Quaternion.identity);
    }

}
