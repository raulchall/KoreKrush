using UnityEngine;

using KoreKrush;


public class TileController : MonoBehaviour
{
    public int color;
    public bool selected = false;
    public Board.Cell cell;

    public int Row { get { return cell.row; } }
    public int Col { get { return cell.col; } }

    public bool AdjacentTo(TileController other)
    {
        return cell.AdjacentTo(other.cell);
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
