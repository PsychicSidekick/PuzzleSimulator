using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridSizeSlider : MonoBehaviour
{
    public void OnValueChange(TMP_Text valueText)
    {
        valueText.text = GetComponent<Slider>().value.ToString();
    }
}
