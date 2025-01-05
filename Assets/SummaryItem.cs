using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SummaryItem : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGui;
    public string itemName;

    public void UpdateDisplay(int amount)
    {
        textMeshProUGui.SetText(amount.ToString() + "x");
    }
}
