using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    [HideInInspector]
    public Orb orb;
    public Vector2Int pos;

    public static Orb selectedOrb;
    public static Cell currentCellMouseIsOn;
    public GameObject orbOnCursorPrefab;

    // Moves an orb from startCell to this Cell one Cell at a time
    public void MoveOrbFrom(Cell startCell)
    {
        // Tracks which cell the orb is currently at
        Cell currCell = startCell;

        // Loops to move orb one cell at a time until the orb has arrived this cell
        while (currCell.pos != pos)
        {
            // Points the position of the cell that the orb needs to go to
            Vector2Int targetPos = new Vector2Int(currCell.pos.x, currCell.pos.y);

            // Moves pointer in x Axis
            if (currCell.pos.x != pos.x)
            {
                targetPos.x += pos.x > startCell.pos.x ? 1 : -1;
            }

            // Moves pointer in y Axis
            if (currCell.pos.y != pos.y)
            {
                targetPos.y += pos.y > startCell.pos.y ? 1 : -1;
            }

            Cell targetCell = GameManager.instance.grid[targetPos.x][targetPos.y];
            targetCell.SwapOrbWith(currCell);

            // Sets the targetCell as the currCell for the next iteration;
            currCell = targetCell;
        }
    }

    private void SwapOrbWith(Cell cell)
    {
        StartCoroutine(MoveOrb(cell));
        Cell endCell = this;
        Orb temp = cell.orb;
        cell.orb = endCell.orb;
        endCell.orb = temp;
    }

    private IEnumerator MoveOrb(Cell cell)
    {
        Orb oldOrb = orb;
        Orb newOrb = cell.orb;
        oldOrb.transform.position = (transform.position + cell.transform.position) / 2;
        newOrb.transform.position = (transform.position + cell.transform.position) / 2;
        yield return new WaitForSeconds(0.075f);
        oldOrb.transform.position = cell.transform.position;
        newOrb.transform.position = transform.position;
        yield return null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameManager.instance.ExpandEdgeCellColliders();

        GameManager.instance.ResetGrid();

        currentCellMouseIsOn = this;
        selectedOrb = orb;

        GameObject orbOnCursor = Instantiate(orbOnCursorPrefab, transform.position, Quaternion.identity);
        GameManager.instance.orbOnCursor = orbOnCursor;
        GameManager.instance.orbOnCursor.GetComponent<SpriteRenderer>().color = orb.spriteRenderer.color;

        orb.ChangeAlpha(0.5f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameManager.instance.ResetCellColliders();

        if (GameManager.instance.startedSpinning)
        {
            GameManager.instance.startedSpinning = false;
            GameManager.instance.ResetGrid();
            GameManager.instance.PopCombos();
        }

        Destroy(GameManager.instance.orbOnCursor);

        selectedOrb.ChangeAlpha(1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.instance.orbOnCursor)
        {
            GameManager.instance.startedSpinning = true;
            MoveOrbFrom(currentCellMouseIsOn);
            currentCellMouseIsOn = this;
        }
    }
}
