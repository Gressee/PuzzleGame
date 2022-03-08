using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Singleton;

public class GridBackground : Singleton<GridBackground>
{
    [SerializeField]
    GameObject backgroundTilePrefab;

    [SerializeField]
    int gridWidth, gridHeight;

    void Awake()
    {
        // TODO add checks for the vars

        gameObject.transform.position = Vector3.zero;

        // Spawn the grid background tile
        for (int x = 0;  x < gridWidth; x++)
        {
            for (int y = 0;  y < gridHeight; y++)
            {
                GameObject tile = Instantiate(backgroundTilePrefab, Vector3.zero, Quaternion.identity);
                tile.transform.SetParent(gameObject.transform);
                // z = 0 is because this object is already on the corret layer
                tile.transform.localPosition = new Vector3(x, y, 0);
                tile.name = backgroundTilePrefab.name;

            }
        }
    }

    public int GetGridWidth()
    {
        return gridWidth;
    }

    public int GetGridHeight()
    {
        return gridHeight;
    }
}
