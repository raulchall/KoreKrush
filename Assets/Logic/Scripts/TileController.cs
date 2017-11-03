using UnityEngine;

using KoreKrush;


public class TileController : MonoBehaviour
{
    public int color;
    public bool selected = false;
    public Board.Cell cell;

    public int Row { get { return cell.row; } }
    public int Col { get { return cell.col; } }
    public bool IsMovable { get { return true; } }
    
    private SpriteRenderer stateImage;

    public SpriteRenderer Sprite { get; private set; }

    public Color Color
    {
        get { return Sprite.color; }
        set { Sprite.color = value; }
    }

    public Sprite StateImage 
    {
        set { stateImage.sprite = value; }
    }

    private void Awake()
    {
        Sprite = GetComponent<SpriteRenderer>();
        stateImage = transform.GetChild(0)
            .GetComponent<SpriteRenderer>();
    }
    
    private void OnMouseDown()
    {
        KoreKrush.Events.Logic.TileSelect_L(this);
    }

    private void OnMouseEnter()
    {
        if (Input.GetMouseButton(0))
            KoreKrush.Events.Logic.TileSelect_L(this);
    }

    public bool AdjacentTo(TileController other)
    {
        return cell.AdjacentTo(other.cell);
    }
}
