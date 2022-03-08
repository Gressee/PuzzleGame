using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject camObject;

    void Start()
    {
        camObject = gameObject;
    }

    void Update()
    {
        // Get the grid width & height from GameManager and adjust camera
        float width = GameManager.Instance.gridWidth;
        float height = GameManager.Instance.gridHeight;

        // Make cam a bit wider than the grid
        float camWidth = width + 1;
        // This sets the vertical size
        Camera.main.orthographicSize = (camWidth/2)/Camera.main.aspect;

        // (bottom left of grid is at 0,0)
        camObject.transform.position = new Vector3(width/2 - 0.5f, height/2 - 1.5f, 0);
    }
}
