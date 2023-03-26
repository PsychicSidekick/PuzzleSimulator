using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<List<Cell>> grid = new List<List<Cell>>();
    public Vector2Int gridSize;
    public GameObject orbPrefab;
    public GameObject cellPrefab;
    public GameObject orbOnCursorPrefab;

    public bool startedSpinning = false;
    public GameObject orbOnCursor;

    public GameObject wall;

    public bool timerStarted = false;
    public float allowedTime = 10;
    public float remainingTime;
    public int comboCount;

    public Slider timerSlider;
    public TMP_Text comboCountTxt;
    public TMP_InputField allowedTimeInput;


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

        if (timerStarted)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0)
            {
                PutOrbOnCursorDown();
                timerStarted = false;
                remainingTime = allowedTime;

            }
        }

        comboCountTxt.text = comboCount.ToString();
        timerSlider.value = remainingTime / allowedTime;
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
                cell.pos = new Vector2Int(x, y);
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
        orb.RandomizeType();
        return orb;
    }

    public void RandomizeGrid()
    {
        for (int x = 0; x < grid.Count; x++)
        {
            for (int y = 0; y < grid[0].Count; y++)
            {
                grid[x][y].orb.RandomizeType();
            }
        }
    }

    // Randomize orb types in grid but ensures no combo exists
    public void RandomizeGridNoCombo()
    {
        do
        {
            RandomizeGrid();
        }
        while (FindAllCombosInGrid().Count > 0);
    }

    // Reset properties of every orbs on the grid
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

    // Destroy all orbs in a combo
    public void PopCombo(List<Orb> combo)
    {
        foreach (Orb orb in combo)
        {
            Destroy(orb.gameObject);
        }
    }

    // Starts PopCombosRoutine
    public void PopCombos()
    {
        StartCoroutine(PopCombosRoutine());
    }

    // Repeats dropping orbs and popping combos until no more combos are found
    private IEnumerator PopCombosRoutine()
    {
        // Prevents orbs from being clicked on
        wall.SetActive(true);

        // While a combo exists on the grid
        do
        {
            List<List<Orb>> combos = FindAllCombosInGrid();

            // Destroy all orbs that are a part of a combo
            foreach (List<Orb> combo in combos)
            {
                comboCount++;
                PopCombo(combo);
                // Separates combo pops
                yield return new WaitForSeconds(0.3f);
            }

            DropRemainingOrbs();
            DropRandomNewOrbs();

            // Waits until all orbs are dropped in place
            yield return new WaitForSeconds(0.4f);

            // Resets grid for next combo find
            ResetGrid();
        }
        while (FindAllCombosInGrid().Count > 0);

        // Allows orbs to be clicked on and start the next spin
        wall.SetActive(false);
        yield return null;
    }

    // Drops the remaining orbs to the bottom
    public void DropRemainingOrbs()
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

    // Creates and drops a new random orb on each column for each free cell on the grid
    public void DropRandomNewOrbs()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            float yPosOffSet = 1;
            for (int y = 0; y < gridSize.y; y++)
            {
                if(!grid[x][y].orb)
                {
                    float xPos = grid[x][y].transform.position.x;
                    // Finds the y position of the new orb by adding the offset to the current column's highest cell's position
                    float yPos = grid[x][gridSize.y-1].transform.position.y + yPosOffSet;
                    // Increments offset so that the next new orb's position will be higher
                    yPosOffSet++;

                    Orb orb = Instantiate(orbPrefab, new Vector2(xPos, yPos), Quaternion.identity).GetComponent<Orb>();
                    orb.pos = new Vector2Int(x, gridSize.y);
                    orb.RandomizeType();

                    orb.Drop();
                }
            }
        }
    }

    public void StartTimer()
    {
        if (timerStarted)
        {
            return;
        }
        timerStarted = true;
        comboCount = 0;
        remainingTime = allowedTime;
    }

    public void PickUpOrb(Orb orb)
    {
        ExpandEdgeCellColliders();

        ResetGrid();

        GameObject o = Instantiate(orbOnCursorPrefab, transform.position, Quaternion.identity);
        orbOnCursor = o;
        orbOnCursor.GetComponent<SpriteRenderer>().color = orb.spriteRenderer.color;

        orb.ChangeAlpha(0.5f);
    }

    public void PutOrbOnCursorDown()
    {
        timerStarted = false;
        ResetCellColliders();

        if (startedSpinning)
        {
            startedSpinning = false;
            ResetGrid();
            PopCombos();
        }

        Destroy(orbOnCursor);

        Cell.selectedOrb.ChangeAlpha(1f);
    }

    public void UpdateAllowedTime()
    {
        allowedTime = int.Parse(allowedTimeInput.text);
    }
}
