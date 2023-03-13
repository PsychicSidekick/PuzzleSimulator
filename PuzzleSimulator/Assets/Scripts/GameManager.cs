using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public List<List<Orb>> grid = new List<List<Orb>>();
    public Vector2Int gridSize;

    public GameObject orbPrefab;

    public void Start()
    {
        InitializeGrid();
    }

    public void InitializeGrid()
    {
        Vector3 firstOrbPos = new Vector3(-0.5f - gridSize.x / 2, gridSize.y / 2, 0);

        for (int x = 0; x < gridSize.x; x++)
        {
            List<Orb> roll = new List<Orb>();

            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 posOffSet = new Vector3(x, -y, 0);
                Orb orb = Instantiate(orbPrefab, firstOrbPos + posOffSet, Quaternion.identity).GetComponent<Orb>();
                roll.Add(orb);
            }

            grid.Add(roll);
        }

        RandomizeGrid();
    }

    public void RandomizeGrid()
    {
        for (int x = 0; x < grid.Count; x++)
        {
            for (int y = 0; y < grid[0].Count; y++)
            {
                // Set a random type for the orb created
                grid[x][y].type = (OrbType)UnityEngine.Random.Range(0, Enum.GetNames(typeof(OrbType)).Length);
            }
        }
    }
}
