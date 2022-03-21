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
        // Returns if the scene could be loaded
        if (DoesSceneExist(sceneName))
        {
            Debug.Log($"Change Scene to \"{sceneName}\"");
            StartCoroutine(LoadLevelCoroutine(sceneName));
            return true;
        }
        else
        {
            Debug.LogWarning($"Scene \"{sceneName}\" doesn't exist");
            return false;
        }
    }

    IEnumerator LoadLevelCoroutine(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);

    }

    public bool DoesSceneExist(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            var lastSlash = scenePath.LastIndexOf("/");
            var sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);

            if (string.Compare(name, sceneName, true) == 0)
                return true;
        }

        return false;
    }
    

}
