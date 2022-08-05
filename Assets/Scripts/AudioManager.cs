using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Singleton;
using Shared.Defines;

public class AudioManager : Singleton<AudioManager>
{
    
    [SerializeField]
    AudioClip btnClick;

    void Start()
    {
        DontDestroyOnLoad(gameObject);        
    }

    void Play(Sound sound)
    {
        // Plays a sound
        switch (sound)
        {
            case Sound.BtnClick: btnClick.PlayOneShot(); break;
        }

    }

}
