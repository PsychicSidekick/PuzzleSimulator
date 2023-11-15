using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadGridButton : MonoBehaviour
{
    public Slider xSlider;
    public Slider ySlider;

    public void OnClick()
    {
        GameManager.instance.gridSize = new Vector2Int((int)xSlider.value, (int)ySlider.value);
        foreach (Cell cell in FindObjectsOfType(typeof(Cell)))
        {
            Destroy(cell.orb.gameObject);
            Destroy(cell.gameObject);
        }
        GameManager.instance.InitializeGrid();
    }
}
