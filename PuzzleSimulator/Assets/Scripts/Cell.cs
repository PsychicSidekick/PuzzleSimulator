using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    [HideInInspector]
    public Orb orb;

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
        GameManager.instance.currentCell = gameObject;
        GameManager.instance.currentOrb = orb;

        GameObject orbOnCursor = Instantiate(orbOnCursorPrefab, transform.position, Quaternion.identity);
        GameManager.instance.orbOnCursor = orbOnCursor;
        orbOnCursor.GetComponent<SpriteRenderer>().color = orb.spriteRenderer.color;

        orb.ChangeAlpha(0.5f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Destroy(GameManager.instance.orbOnCursor);

        GameManager.instance.currentOrb.ChangeAlpha(1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.instance.orbOnCursor)
        {
            SwapOrbs(GameManager.instance.currentCell.GetComponent<Cell>());
            GameManager.instance.currentCell = gameObject;
        }
    }
}
