using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public int row;
    public int col;
    public int color;
    public bool selected = false;

    public bool AdjacentTo(TileController other)
    {
        return Mathf.Abs(row - other.row) + Mathf.Abs(col - other.col) == 1
            || Mathf.Abs(row - other.row) == 1 && Mathf.Abs(col - other.col) == 1;
    }

    void OnMouseDown()
    {
        KoreKrush.Events.Logic.TileSelected(this);
    }
//
//    void OnMouseEnter()
//    {
//        if (Input.GetMouseButton (0)) {
//            if (tilesManager.TryAddTile (this)) {
//                Select ();
//            } else {
//                MarkAsWrong ();
//            }
//        }
//    }
//
//    void OnMouseExit()
//    {
//        if (!selected)
//            ClearMark ();
//    }
//
//    public void SetUp(int i, int j, Color[] colors, TilesManagerController tilesManager)
//    {
//        row = i;
//        col = j;
//        sprite.color = colors.Choice ();
//        originalColor = sprite.color;
//        this.tilesManager = tilesManager;
//    }
//
//    public bool SameColor(TileController other)
//    {
//        return other.sprite.color == sprite.color;
//    }
//
//    public void Select()
//    {
//        selected = true;
//        MarkAsRight ();
//    }
//
//    public void Unselect()
//    {
//        selected = false;
//        ClearMark ();
//    }
//
//    public void ChangeColor(Color newColor)
//    {
//        sprite.color = newColor;
//    }
//
//    private void MarkAsRight()
//    {
//        stateImage.sprite = tilesManager.rightImage;
//    }
//
//    private void MarkAsWrong()
//    {
//        stateImage.sprite = tilesManager.wrongImage;
//    }
//
//    private void ClearMark()
//    {
//        stateImage.sprite = null;
//    }
//
}
