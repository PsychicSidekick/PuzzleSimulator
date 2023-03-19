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
        if (orbOnCursor != null)
        {
            orbOnCursor.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
        }
    }

    public void InitializeGrid()
    {
        Vector3 firstOrbPos = new Vector3(0.5f - gridSize.x / 2f, 0.5f - gridSize.y / 2f, 0);

        for (int x = 0; x < gridSize.x; x++)
        {
            List<Cell> roll = new List<Cell>();

            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 posOffSet = new Vector3(x, y, 0);
                Orb orb = InstantiateRandomOrb(firstOrbPos + posOffSet, new Vector2Int(x, y));
                Cell cell = Instantiate(cellPrefab, firstOrbPos + posOffSet, Quaternion.identity).GetComponent<Cell>();
                cell.orb = orb;
                roll.Add(cell);
            }

            grid.Add(roll);
        }

        RandomizeGridNoCombo();
    }

    public List<List<Orb>> FindAllCombosInGrid()
    {
        List<List<Orb>> combos = new List<List<Orb>>();
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
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

        ResetGrid();
        return combos;
    }

    public void ExpandEdgeCellColliders()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            BoxCollider2D collider = grid[x][0].GetComponent<BoxCollider2D>();
            collider.offset += new Vector2(0, -4.5f);
            collider.size += new Vector2(0, 9);

            collider = grid[x][gridSize.y - 1].GetComponent<BoxCollider2D>();
            collider.offset += new Vector2(0, 4.5f);
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

    public void RandomizeGridNoCombo()
    {
        do
        {
            RandomizeGrid();
        }
        while (FindAllCombosInGrid().Count > 0);
    }

    public void ResetGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Orb orb = grid[x][y].orb;
                orb.pos = new Vector2Int(x, y);
                orb.isChecked = false;
                orb.dropping = false;
                orb.dropTargetPos = grid[x][y].orb.transform.position;
                orb.dropSpeed = 0;
            }
        }
    }

    public void PopCombo(List<Orb> combo)
    {
        foreach (Orb orb in combo)
        {
            Destroy(orb.gameObject);
        }
    }

    public void PopCombos()
    {
        List<List<Orb>> combos = FindAllCombosInGrid();
        StartCoroutine(PopCombosRoutine(combos));
    }

    private IEnumerator PopCombosRoutine(List<List<Orb>> combos)
    {
        foreach (List<Orb> combo in combos)
        {
            PopCombo(combo);
            yield return new WaitForSeconds(0.3f);
        }

        DropExistingOrbs();
        Debug.Log(combos.Count);
        yield return null;
    }

    public void DropExistingOrbs()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (grid[x][y].orb)
                {
                    grid[x][y].orb.Drop();
                }
            }
        }
    }
}
