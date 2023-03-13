using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<List<Cell>> grid = new List<List<Cell>>();
    public Vector2Int gridSize;
    public GameObject orbPrefab;
    public GameObject cellPrefab;

    public GameObject orbOnCursor;
    public Orb currentOrb;
    public GameObject currentCell;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        InitializeGrid();
    }

    private void Update()
    {
        if (currentCell)
        {
            Debug.Log(currentCell.transform.position);
        }
        

        if (orbOnCursor != null)
        {
            orbOnCursor.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
        }
    }

    public void InitializeGrid()
    {
        Vector3 firstOrbPos = new Vector3(0.5f - gridSize.x / 2f, gridSize.y / 2f - 0.5f, 0);

        for (int x = 0; x < gridSize.x; x++)
        {
            List<Cell> roll = new List<Cell>();

            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 posOffSet = new Vector3(x, -y, 0);
                Orb orb = InstantiateRandomOrb(firstOrbPos + posOffSet, new Vector2Int(x, y));
                Cell cell = Instantiate(cellPrefab, firstOrbPos + posOffSet, Quaternion.identity).GetComponent<Cell>();
                cell.orb = orb;
                roll.Add(cell);
            }

            grid.Add(roll);
        }

        ExpandEdgeCellCollider();
    }

    public void ExpandEdgeCellCollider()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            BoxCollider2D collider = grid[x][0].GetComponent<BoxCollider2D>();
            collider.offset += new Vector2(0, 4.5f);
            collider.size += new Vector2(0, 9);

            collider = grid[x][gridSize.y - 1].GetComponent<BoxCollider2D>();
            collider.offset += new Vector2(0, -4.5f);
            collider.size += new Vector2(0, 9);
        }

        for (int y = 0; y < gridSize.y; y++)
        {
            BoxCollider2D collider = grid[0][y].GetComponent<BoxCollider2D>();
            collider.offset += new Vector2(-4.5f, 0);
            collider.size += new Vector2(9, 0);

            collider = grid[gridSize.x - 1][y].GetComponent<BoxCollider2D>();
            collider.offset += new Vector2(4.5f, 0);
            collider.size += new Vector2(9, 0);
        }
    }

    public Orb InstantiateRandomOrb(Vector3 worldPos, Vector2Int gridPos)
    {
        Orb orb = Instantiate(orbPrefab, worldPos, Quaternion.identity).GetComponent<Orb>();
        orb.pos = gridPos;
        RandomizeOrbType(orb);
        return orb;
    }

    public void RandomizeOrbType(Orb orb)
    {
        orb.type = (OrbType)UnityEngine.Random.Range(0, Enum.GetNames(typeof(OrbType)).Length);
    }

    public void RandomizeGrid()
    {
        for (int x = 0; x < grid.Count; x++)
        {
            for (int y = 0; y < grid[0].Count; y++)
            {
                RandomizeOrbType(grid[x][y].orb);
            }
        }
    }

    public void CheckForComboAt(int x, int y)
    {
        int xPointer = x;

        // check to the right
        while(xPointer < gridSize.x)
        {
            xPointer++;

        }

        xPointer = x;

        // check to the left
        while(xPointer >= 0)
        {
            xPointer--;

        }
    }
}
