using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineIndicator : MonoBehaviour
{
    [HideInInspector]
    public List<int> columnIndexes = new List<int>();
    public List<int[]> line_data = new List<int[]>();

    private Grid _grig;
    
    
    public void ColumnIndexesGeneric()
    {
        for (int i = 0; i < _grig.columns - 1; i++)
        {
            columnIndexes.Add(i);
        }
    }

    private int[] LineDataRowGeneric(int value)
    {
        int[] line_data_row = new int[_grig.columns];
        for (int column = 0; column < _grig.columns; column++)
            {
                line_data_row[column] = value;
                value++;
            }
        return line_data_row;
    }

    private void LineDataGeneric()
    {
        int value = 0;
        for (int row = 0; row < _grig.rows; row++)
        {
            line_data.Add(LineDataRowGeneric(value));
            value += _grig.rows;
            
        }
    }

    private void Start() {
        _grig = GetComponent<Grid>();
        ColumnIndexesGeneric();
        LineDataGeneric();
        
    }

    private (int, int) GetSquarePosition(int square_index)
    {
        int pos_row = -1;
        int pos_column = -1;
        
        for (int row = 0; row < _grig.rows; row++)
        {
            for (int column = 0; column < _grig.columns; column++)
            {
                if (line_data[row][column] == square_index)
                {
                    pos_row = row;
                    pos_column = column;
                }
            }
        }

        return (pos_row, pos_column);
    }

    public int[] GetVerticalLine(int square_index)
    {
        int[] line = new int[_grig.columns];
        var square_position_column = GetSquarePosition(square_index).Item2;

        for (int index = 0; index < _grig.columns; index++)
        {
            line[index] = line_data[index][square_position_column];
        }

        return line;
    }
}
