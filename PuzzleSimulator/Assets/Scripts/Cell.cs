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

    // Swap orbs with another cell
    private void SwapOrbWith(Cell startCell)
    {
        float swapSpeed = 2500;

        // Determines the direction of rotation 
        Vector2 posDiff = startCell.transform.position - transform.position;
        if (posDiff.x != 0)
        {
            swapSpeed *= posDiff.x;
        }
        else if (posDiff.y != 0)
        {
            swapSpeed *= posDiff.y;
        }

        // Swaps the orb gameObjects
        startCell.orb.StartSwap(startCell.transform.position, transform.position, swapSpeed);
        orb.StartSwap(transform.position, startCell.transform.position, swapSpeed);

        // Swaps orb objects
        Orb temp = startCell.orb;
        startCell.orb = orb;
        orb = temp;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerId != -1)
        {
            return;
        }

        currentCellMouseIsOn = this;
        selectedOrb = orb;

        GameManager.instance.PickUpOrb(orb);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != -1)
        {
            return;
        }

        if (!GameManager.instance.orbOnCursor)
        {
            return;
        }

        GameManager.instance.PutOrbOnCursorDown();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.instance.orbOnCursor)
        {
            GameManager.instance.StartTimer();
            orb.transform.position = transform.position;
            GameManager.instance.startedSpinning = true;
            // Move the orb from previous cell to this cell
            MoveOrbFrom(currentCellMouseIsOn);
            currentCellMouseIsOn = this;
        }
    }
}
