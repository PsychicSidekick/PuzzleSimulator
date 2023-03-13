using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject gridAnchor;

    public Vector2Int gridSize;

    public GameObject orbPrefab;

    public void Start()
    {
        InitializeGrid();
    }

    public void InitializeGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 posOffSet = new Vector3(x, -y, 0);
                Instantiate(orbPrefab, gridAnchor.transform.position + posOffSet, Quaternion.identity);
            }
        }
    }
}
