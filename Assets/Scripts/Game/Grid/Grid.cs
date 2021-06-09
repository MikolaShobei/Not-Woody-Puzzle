using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Grid : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public static int BigRow = 9;
    public static int BigColumn = 9;

    [HideInInspector]
    public int rows = 9;

    [HideInInspector]
    public int columns = 9;
    public float squaresGab = 0.0f;
    public GameObject gridSquare;

    [HideInInspector]
    public Vector2 startPosition = new Vector2(0.0f, 0.0f);
    public float everySquareOffset = 0.0f;
    public float squareSkale = 1.0f;

    private Vector2 _offset = new Vector2(0.0f, 0.0f);
    private List<GameObject> _gridSQuares = new List<GameObject>();

    private LineIndicator _lineIndicator;

    private void OnEnable() {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
        
    }

    private void OnDisable() {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
    }
    
    private void Awake() {
        rows = BigRow;
        columns = BigColumn;

        SetStartPosision();
    }

    void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
        CreateGrid();

    }

    private void SetStartPosision()
    {
        startPosition.x = -1 * ((rows - 1) * 155) / 4;  
        startPosition.y = -1 * ((columns - 1) * 155) / 4;  
    }

   

    private void CreateGrid(){
        ChildsGridSquares();
        SetGridSquaresPositons();

    }

    private void ChildsGridSquares(){
        int square_index = 0;
        for(int row = 0; row < rows; ++row){
            for(int column = 0; column < columns; ++column){
                _gridSQuares.Add(Instantiate(gridSquare) as GameObject);

                _gridSQuares[_gridSQuares.Count - 1].GetComponent<GridSquare>().SquareIndex = square_index;
                _gridSQuares[_gridSQuares.Count - 1].transform.SetParent(this.transform);
                _gridSQuares[_gridSQuares.Count - 1].transform.localScale = new Vector3(squareSkale, squareSkale, squareSkale);
                _gridSQuares[_gridSQuares.Count - 1].GetComponent<GridSquare>().SetImage(square_index % 2 == 0);
                square_index++;
            }
        }
    }

    private void SetGridSquaresPositons(){
        int rowNumber = 0;
        int columnNumber = 0;
        Vector2 squareGapNumber = new Vector2(0.0f, 0.0f);

        var squareRect = _gridSQuares[0].GetComponent<RectTransform>();
        _offset.x = squareRect.rect.width * squareRect.transform.localScale.x + everySquareOffset;
        _offset.y = squareRect.rect.width * squareRect.transform.localScale.y + everySquareOffset;

        foreach (GameObject square in _gridSQuares){
            if (columnNumber + 1 > columns){
                columnNumber = 0;
                squareGapNumber.x = 0;
                rowNumber++;
            }
            var posXOffset = _offset.x * columnNumber + (squareGapNumber.x * squaresGab);
            var posYOffset = _offset.y * rowNumber + (squareGapNumber.y * squaresGab);

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosition.x + posXOffset, startPosition.y + posYOffset);
            square.GetComponent<RectTransform>().localPosition = new Vector3(startPosition.x + posXOffset, startPosition.y + posYOffset, 0.0f);
            
            columnNumber++;
        }
    }

    private void CheckIfShapeCanBePlaced(){
        var squareIndexes = new List<int>();

        foreach (var square in _gridSQuares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if (gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                squareIndexes.Add(gridSquare.SquareIndex);
                gridSquare.Selected = false;
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if(currentSelectedShape == null) return;

        if (currentSelectedShape.TotalSquareNumber == squareIndexes.Count)
        {
            foreach (var squareIndex in squareIndexes)
            {
                _gridSQuares[squareIndex].GetComponent<GridSquare>().PlaceShapeOnBoard();
            }

            var shapeLeft = 0;

            foreach (var shape in shapeStorage.shapeList)
            {
                if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareAcrive())
                {
                    shapeLeft++;
                }
            }

            if (shapeLeft == 0)
            {
                GameEvents.RequestNewShapes();
            }
            else
            {
                GameEvents.SetShapeInactive();
            }

            ChackIfAnyLineIsCompleted();
        }   
        else
        {
            GameEvents.MoveShapeToStartPosition();
        }
    }

    private void ChackIfAnyLineIsCompleted()
    {
        List<int[]> lines = new List<int[]>();

        foreach (var column in _lineIndicator.columnIndexes)
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));
        }

        for (int row = 0; row < rows; row++) 
        {
            List<int> data = new List<int>(columns);
            for (int column = 0; column < columns; column++)
            {
                data.Add(_lineIndicator.line_data[row][column]); // Changed
            }

            lines.Add(data.ToArray());
        }

        var completedLines = CheckIfSquaresAreCompleted(lines);

        //  Scores
        var totalScores = completedLines;
        GameEvents.AddScores(totalScores);

        CheckIfPlayerLost();

    }

    private int CheckIfSquaresAreCompleted(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();

        var linesCompleted = 0;

        foreach (var line in data)
        {
            var lineCompleted = true;
            foreach (var squareIndex in line)
            {
                var comp = _gridSQuares[squareIndex].GetComponent<GridSquare>();
                if (comp.SquareOccupied == false)
                {
                    lineCompleted = false;
                }
            }

            if (lineCompleted)
            {
                completedLines.Add(line);
            }
        }

        foreach (var line in completedLines)
        {
            var completed = false;

            foreach (var squareIndex in line)
            {
                var comp = _gridSQuares[squareIndex].GetComponent<GridSquare>();
                comp.Deactivate();
                completed = true;
            }

            foreach (var squareIndex in line)
            {
                var comp = _gridSQuares[squareIndex].GetComponent<GridSquare>();            
                comp.ClearOccupied();
            }

            if (completed)
            {
                linesCompleted++;
            }

        }
        return linesCompleted;
    }

    private void CheckIfPlayerLost()
    {
        var validShapes = 0;

        for (int index = 0; index < shapeStorage.shapeList.Count; index++)
        {
            var isShapeActive = shapeStorage.shapeList[index].IsAnyOfShapeSquareAcrive();

            if (CheckIfShapeCanBePlacedOnGrid(shapeStorage.shapeList[index]) && isShapeActive)
            {
                shapeStorage.shapeList[index]?.ActivateShape();
                validShapes++;
            }
        }

        if (validShapes == 0)
        {
            // GAME OVER
            GameEvents.GameOver(false);
            Debug.LogError("Game is over");
        }
    }

    private bool CheckIfShapeCanBePlacedOnGrid(Shape currentShape)
    {
        var currentShapeData = currentShape.CurrentShapeData;
        var shapeColumns = currentShapeData.columns;
        var shapeRows = currentShapeData.rows;

        List<int> originalShapeFilledUpSquares = new List<int>();
        var squareIndex = 0;

        for (int rowIndex = 0; rowIndex < shapeRows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < shapeColumns; columnIndex++)
            {
                if (currentShapeData.board[rowIndex].column[columnIndex])
                {
                    originalShapeFilledUpSquares.Add(squareIndex);
                }

                squareIndex++;
            }
        }

        if (currentShape.TotalSquareNumber != originalShapeFilledUpSquares.Count)
            Debug.LogError("Vsio pizdet`s");         
        
        var squareList = GetAllSquaresCombination(shapeColumns, shapeRows);

        bool canBePlased = false;

        foreach (var number in squareList)
        {
            bool shapeCanBePlacedOnTheBoard = true;
            foreach (var squareIndexToChack in originalShapeFilledUpSquares)
            {
                var comp = _gridSQuares[number[squareIndexToChack]].GetComponent<GridSquare>();

                if (comp.SquareOccupied)
                {
                    shapeCanBePlacedOnTheBoard = false;
                }
            }

            if (shapeCanBePlacedOnTheBoard)
            {
                canBePlased = true;
            }
        }

        return canBePlased;
    }

    private List<int[]> GetAllSquaresCombination(int rows_local, int columns_local)
    {
        var squareList = new List<int[]>();
        var lastColumnIndex = 0;
        var lastRowIndex = 0;

        int safeIndex = 0;

        while (lastRowIndex + (rows_local - 1) < rows)
        {
            var rowData = new List<int>();

            for (int row = lastRowIndex; row < lastRowIndex + rows_local; row++)
            {
                for (int column = lastColumnIndex; column < lastColumnIndex + columns_local; column++)
                {
                    rowData.Add(_lineIndicator.line_data[row][column]);
                }
            }

            squareList.Add(rowData.ToArray());

            lastColumnIndex++;

            if (lastColumnIndex + (columns_local - 1) >= columns)
            {
                lastRowIndex++;
                lastColumnIndex = 0;
            }

            safeIndex++;

            if (safeIndex > 250)
            {
                break;
            }
        
        }

        return squareList;

    }
}
