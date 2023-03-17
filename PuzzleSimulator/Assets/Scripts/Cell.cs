using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    [HideInInspector]
    public Orb orb;

    public static Orb selectedOrb;
    public static Cell currentCellMouseIsOn;
    public GameObject orbOnCursorPrefab;

    public void SwapOrbs(Cell cell)
    {
        Orb temp = cell.orb;
        cell.orb = orb;
        cell.orb.transform.position = cell.transform.position;
        orb = temp;
        temp.transform.position = transform.position;
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
        }

        Destroy(GameManager.instance.orbOnCursor);

        selectedOrb.ChangeAlpha(1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.instance.orbOnCursor)
        {
            GameManager.instance.startedSpinning = true;
            SwapOrbs(currentCellMouseIsOn);
            currentCellMouseIsOn = this;
        }
    }
}
