using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{   
    public Image shadowImage;
    public Image activeImage;
    public Image initialImage;
    public List<Sprite> initialImages;
    // Start is called before the first frame update
    public bool Selected { get; set; }
    public int SquareIndex { get; set; }
    public bool SquareOccupied { get; set; }
    void Start()
    {
        Selected = false;
        SquareOccupied = false;
    }

    public bool CanWeUseThisSquare()
    {
        return shadowImage.gameObject.activeSelf;
    }

    public void PlaceShapeOnBoard()
    {
        ActivateSquare();
    }

    public void ActivateSquare()
    {
        shadowImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
        Selected = true;
        SquareOccupied = true;
    }

    public void Deactivate()
    {
        activeImage.gameObject.SetActive(false);
    }

    public void ClearOccupied()
    {
        Selected = false;
        SquareOccupied = false;
    }
    
    public void SetImage(bool isFirstTrue){
        initialImage.GetComponent<Image>().sprite = isFirstTrue ? initialImages[0] : initialImages[1];
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if (SquareOccupied == false)
        {
            Selected = true;
            shadowImage.gameObject.SetActive(true);
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }
    }

    private void OnTriggerStay2D(Collider2D collision){
        Selected = true;
        if (SquareOccupied == false)
        {
            shadowImage.gameObject.SetActive(true);
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }

    }

    private void OnTriggerExit2D(Collider2D collision){
        if (SquareOccupied == false)
        {
            Selected = false;
            shadowImage.gameObject.SetActive(false);
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().UnSetOccupied();
        }
            
    }

}
