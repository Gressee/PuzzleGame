using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Singleton;

public class SceneTransManager : Singleton<SceneTransManager>
{
    [SerializeField]
    public Animator transition;

    [SerializeField]
    public float transitionTime;

    public bool LoadScene(string sceneName)
    {
        // Returns if the scene exists
        StartCoroutine(LoadLevelCoroutine(sceneName));
        return true;
    }

    IEnumerator LoadLevelCoroutine(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);

    }
    

}
