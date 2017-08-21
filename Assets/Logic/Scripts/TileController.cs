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
        KoreKrush.Events.Logic.TileSelected_L(this);
    }

    void OnMouseEnter()
    {
        if (Input.GetMouseButton(0))
            KoreKrush.Events.Logic.TileSelected_L(this);
    }
}
