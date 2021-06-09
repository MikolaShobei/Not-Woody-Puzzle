using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler,  IEndDragHandler, IPointerDownHandler
{
    public GameObject shapeSquareImage;
    public Vector3 shapeSelectedScale;
    public Vector2 offset = new Vector2(0f, 700f);

    [HideInInspector]
    public ShapeData CurrentShapeData;

    public int TotalSquareNumber { get; set; }
    
    private List<GameObject> _currentShapes = new List<GameObject>();
    private Vector3 _shapeStartScale;
    private RectTransform _transform;
    private Canvas _canvas;
    private Vector3 _startPosition;
    private bool _shapeActie = true;
    
    public void Awake()
    {
        _shapeStartScale = this.GetComponent<RectTransform>().localScale;
        _transform = this.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _startPosition = _transform.localPosition;
        _shapeActie = true;
    }

    private void OnEnable() {
        GameEvents.MoveShapeToStartPosition += MoveShapeToStartPosition;
        GameEvents.SetShapeInactive += SetShapeInactive;

    }

    private void OnDisable() {
        GameEvents.MoveShapeToStartPosition -= MoveShapeToStartPosition;
        GameEvents.SetShapeInactive -= SetShapeInactive;

    }

    public bool IsOnStartPosition()
    {
        return _transform.localPosition == _startPosition;
    }

    public bool IsAnyOfShapeSquareAcrive()
    {
        foreach (var square in _currentShapes)
        {
            if (square.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
    public void DeactivateShape()
    {
        if (_shapeActie)
        {
            foreach (var square in _currentShapes)
            {
                square?.GetComponent<ShapeSquare>().DeactivateShape();
            }
            _shapeActie = false;
        }
    }

    public void SetShapeInactive()
    {
        if (IsAnyOfShapeSquareAcrive() && IsOnStartPosition() == false)
        {
            foreach (var square in _currentShapes)
            {
                square.gameObject.SetActive(false);
            }
        }
    }

    public void ActivateShape()
    {
        if (!_shapeActie)
        {
            foreach (var square in _currentShapes)
            {
                square?.GetComponent<ShapeSquare>().ActivateShape();
            }
            _shapeActie = true;
        }
    }
    
    public void RequestNewShape(ShapeData shapeData)
    {   
        _transform.localPosition = _startPosition;
        CreateShape(shapeData);
    }

    public void CreateShape (ShapeData shapeData)
    {
        CurrentShapeData = shapeData;
        TotalSquareNumber = GetNumberOfSquares(shapeData);

        while (_currentShapes.Count <= TotalSquareNumber)
        {
            _currentShapes.Add(Instantiate(shapeSquareImage, transform) as GameObject);
        }

        foreach (var square in _currentShapes)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var squareRect = shapeSquareImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x, squareRect.rect.height * squareRect.localScale.y);
        int currentIndexInList = 0;
        for (int row = 0; row < shapeData.rows; row++)
        {
            for (int column = 0; column < shapeData.columns; column++)
            {
                if (shapeData.board[row].column[column])
                {
                    _currentShapes[currentIndexInList].SetActive(true);
                    _currentShapes[currentIndexInList].GetComponent<RectTransform>().localPosition = 
                    new Vector2(GetXPositionInShapeSquare(shapeData, column, moveDistance),
                    GetYPositionInShapeSquare(shapeData, row, moveDistance));
                    currentIndexInList++;    
                }
                
            }
        }
    }

    public float GetYPositionInShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance){
        float shiftOnY = 0f;

        if (shapeData.rows > 1)
        {
            if (shapeData.rows % 2 != 0)
            {
                var middleSquareIndex = (shapeData.rows - 1) / 2;
                var multiplier = (shapeData.rows - 1) / 2;

                if (row < middleSquareIndex)
                {
                    shiftOnY = moveDistance.y * 1;
                    shiftOnY *= multiplier;
                }
                else if (row > middleSquareIndex)
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY *= multiplier;
                }
            }
            else
            {
                var middleSquareIndex2 = (shapeData.rows == 2) ? 1 : shapeData.rows / 2;
                var middleSquareIndex1 = (shapeData.rows == 2) ? 0 : shapeData.rows - 2;
                var multiplier = shapeData.rows / 2;
                if (row == middleSquareIndex1 || row == middleSquareIndex2)
                {
                    if (row == middleSquareIndex2)
                        shiftOnY = (moveDistance.y / 2) * -1;
                    if (row == middleSquareIndex1)
                        shiftOnY = moveDistance.y / 2;
                }

                if (row < middleSquareIndex1 && row < middleSquareIndex2)
                {
                    shiftOnY = moveDistance.y * 1;
                    shiftOnY *= multiplier;
                }
                if (row > middleSquareIndex1 && row > middleSquareIndex2)
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY = multiplier;
                }
            }
        }
        return shiftOnY;
    }

    public float GetXPositionInShapeSquare(ShapeData shapeData,int column, Vector2 moveDistance){
        float shiftOnX = 0.0f;

        if(shapeData.columns > 1){
            if (shapeData.columns % 2 != 0)
            {
                var middleSquareIndex = (shapeData.columns - 1) / 2;
                var multiplier = (shapeData.columns - 1) / 2;
                if (column < middleSquareIndex)
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if (column > middleSquareIndex)
                {
                    shiftOnX = moveDistance.x * 1;
                    shiftOnX *= multiplier;
                }
            }
            else
            {
                var middleSquareIndex2 = (shapeData.columns == 2) ? 1 : shapeData.columns / 2;
                var middleSquareIndex1 = (shapeData.columns == 2) ? 0 : shapeData.columns - 1;
                var multiplier = shapeData.columns / 2;
                if (column == middleSquareIndex1 || column == middleSquareIndex2)
                {
                    if (column == middleSquareIndex1)
                        shiftOnX = (moveDistance.x / 2) * -1;
                    if (column == middleSquareIndex2)
                        shiftOnX = moveDistance.x / 2;
                }

                if (column < middleSquareIndex1 && column < middleSquareIndex2)
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                if (column > middleSquareIndex1 && column > middleSquareIndex2)
                {
                    shiftOnX = moveDistance.x * 1;
                    shiftOnX = multiplier;
                }
            }
        }
        return shiftOnX;
    }

    private int GetNumberOfSquares(ShapeData shapeData){
        int number = 0;
        foreach (var row in shapeData.board)
        {
            foreach (var active in row.column)
            {
                if(active)
                    number++;
            }
        }
        return number;
    }

    
    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = shapeSelectedScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _transform.anchorMin = new Vector2(0, 0);
        _transform.anchorMax = new Vector2(0, 0);
        _transform.pivot = new Vector2(0, 0);
        
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, eventData.position, Camera.main, out pos);
        _transform.localPosition = pos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = _shapeStartScale;
        GameEvents.CheckIfShapeCanBePlaced();
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    private void MoveShapeToStartPosition(){
        _transform.transform.localPosition = _startPosition;
    }
}
