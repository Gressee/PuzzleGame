using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BtnLevelPack : MonoBehaviour
{

    [SerializeField]
    char levelPack;

    // This is the parent object of this button
    GameObject levelPackListItem;

    // This is the container of all the Level Buttons 
    GameObject lvlButtonsContainer;
    
    void Start()
    {
        // Add the button function
        gameObject.GetComponent<Button>().onClick.AddListener(ButtonsClick);

        // Get parent
        levelPackListItem = transform.parent.gameObject;

        // Get all children of parent and find the Container 
        // of the level buttons
        if (transform.childCount > 2)
        {
            Debug.LogError("Level Pack Item has more than 2 childs"
                + "Childs should only be \"BtnLevelPack\" and \"LevelButtonsContainer\"");
            return;
        }
        foreach (Transform child in levelPackListItem.transform)
        {
            if (child.gameObject != gameObject)
            {
                // This child must be the the lvl btn container
                lvlButtonsContainer = child.gameObject;
            }
        }

        // Make the level buttons not active if they arent already
        lvlButtonsContainer.SetActive(false);

        // Set the text of the Button
        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = 
            "Level Pack " + levelPack.ToString().ToUpper();
    }

    void ButtonsClick()
    {
        // Toggle the the level buttons container to show them
        lvlButtonsContainer.SetActive(!lvlButtonsContainer.activeSelf);
    }
}
