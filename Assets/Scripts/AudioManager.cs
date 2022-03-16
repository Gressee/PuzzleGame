using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Singleton;

public class AudioManager : Singleton<AudioManager>
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);        
    }

}
