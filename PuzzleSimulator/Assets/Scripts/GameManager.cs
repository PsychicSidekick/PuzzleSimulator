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

    public bool startedSpinning = false;
    public GameObject orbOnCursor;

    public static readonly Vector2Int rightDir = new Vector2Int(1, 0);
    public static readonly Vector2Int leftDir = new Vector2Int(-1, 0);
    public static readonly Vector2Int downDir = new Vector2Int(0, 1);
    public static readonly Vector2Int upDir = new Vector2Int(0, -1);

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        InitializeGrid();
    }

    public void Test()
    {
        ResetGrid();

        List<List<Orb>> combos = new List<List<Orb>>();
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Orb orb = grid[x][y].orb;
                if (!orb.isChecked)
                {
                    List<Orb> combo = grid[x][y].orb.FindComboOrbs();
                    if (combo.Count != 0)
                    {
                        combos.Add(combo);
                    }
                }
            }
        }

        Debug.Log(combos.Count);
        foreach (List<Orb> combo in combos)
        {
            string str = "";
            Debug.Log(combo.Count);
            foreach (Orb orb in combo)
            {
                str += orb.pos;
            }
            Debug.Log(str);
        }
    }

    private void Update()
    {
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
    }

    public void ExpandEdgeCellColliders()
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

    public void ResetCellColliders()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                BoxCollider2D collider = grid[x][y].GetComponent<BoxCollider2D>();
                collider.offset = new Vector2(0, 0);
                collider.size = new Vector2(1, 1);
            }
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

    public void ResetGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                grid[x][y].orb.pos = new Vector2Int(x, y);
                grid[x][y].orb.isChecked = false;
            }
        }
    }
}
