using KoreKrush;
using UnityEngine;


public enum TileType
{
    None,
    Blue,
    Green,
    Yellow,
    Red
}

public class StandardTile : MonoBehaviour
{
    // TODO: Remove this patch
    [HideInInspector]
    public int color;
    
    public TileType TileType = TileType.None;
    [Range(1, 100)]
    public int Value = 1;
    
    [Header("Graphical components")]
    public BaseTileAnimator Animator;
    
    internal Board.Cell Cell;
    internal bool IsConnected;
    internal int Row { get { return Cell.row; } }
    internal int Col { get { return Cell.col; } }
    internal bool IsMovable = true;
    internal SpriteRenderer Sprite;

    private void Awake()
    {
        Sprite = GetComponent<SpriteRenderer>();
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

    public virtual bool IsCompatible(StandardTile other)
    {
        return TileType == other.TileType;
    }

    public virtual bool IsAdjacent(StandardTile other)
    {
        return Cell.AdjacentTo(other.Cell);
    }

    public virtual void Spawn(float animDuration, float animDelay)
    {
        Animator.Spawn(this, animDuration, animDelay);
    }

    public virtual void Move(Vector2 newPos, float animDuration, float animDelay)
    {
        Animator.Move(this, newPos, animDuration, animDelay);
    }

    public virtual void Connect()
    {
        IsConnected = true;
        Animator.Connect(this);
    }

    public virtual void Disconnect()
    {
        IsConnected = false;
        Animator.Disconnect(this);
    }
}
