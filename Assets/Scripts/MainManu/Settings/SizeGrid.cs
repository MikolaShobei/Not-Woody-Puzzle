using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SizeGrid : MonoBehaviour
{
    public Text rowsText;
    public Text columnText;

    [HideInInspector]
    public int currentValue;
   
    void Start()
    {
        currentValue = 9;
        rowsText.text = currentValue.ToString();
        columnText.text = currentValue.ToString();

        Grid.BigRow = currentValue;
        Grid.BigColumn = currentValue;

    }

    public void OnIncrement()
    {
        if (currentValue < 12)
        {
            currentValue += 1;
        }

        Grid.BigRow = currentValue;
        Grid.BigColumn = currentValue;

        rowsText.text = currentValue.ToString();
        columnText.text = currentValue.ToString();
    }

    public void OnDecrement()
    {
        if (currentValue > 5)
        {
            currentValue -= 1;
        }
        
        Grid.BigRow = currentValue;
        Grid.BigColumn = currentValue;

        rowsText.text = currentValue.ToString();
        columnText.text = currentValue.ToString();
    }

}
