using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core.Singleton;
using Shared.Defines;


public class ButtonGeneral : MonoBehaviour
{   

    // This script gets placed in EVERY BUTTON
    // Functions get paced here that should run
    // on every button

    /*
    GET HANDLED HERE:
        Audio (Click sound)

    MAYBE IN THE FUTURE:
        Button Style (color, ...)


    */

    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        AudioManager.Instance.
    }
}
