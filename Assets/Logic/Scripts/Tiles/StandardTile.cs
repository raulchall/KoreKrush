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
    public TileType TileType = TileType.None;
    [Range(1, 100)]
    public int Value = 1;
    
    [Header("Graphical components", order = 100)]
    public BaseTileAnimator Animator;
    
    internal Board.Cell Cell;
    internal bool IsConnected;
    internal bool IsMovable = true;
    internal bool IsTarget { get { return TargetsCount > 0; } }
    internal int Row { get { return Cell.row; } }
    internal int Col { get { return Cell.col; } }
    internal SpriteRenderer Sprite;
    internal GameObject Highlight;

    private int TargetsCount;


    private void Awake()
    {
        Sprite = GetComponent<SpriteRenderer>();
        Highlight = Instantiate(Resources.Load("Tiles/Highlight"), transform) as GameObject;
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

    public virtual void Connect(float animDuration, float animDelay)
    {
        IsConnected = true;
        Animator.Connect(this, animDuration, animDelay);
    }

    public virtual void Disconnect(float animDuration, float animDelay)
    {
        IsConnected = false;
        Animator.Disconnect(this, animDuration, animDelay);
    }

    public virtual void Destroy(float animDuration, float animDelay)
    {
        Animator.Destroy(this, animDuration, animDelay);
        
        Cell.Tile = null;
    }

    public virtual void Aim(float animDuration, float animDelay)
    {
        TargetsCount++;
        Animator.Aim(this, animDuration, animDelay);
    }

    public virtual void Unaim(float animDuration, float animDelay)
    {
        TargetsCount--;
        
        if (!IsTarget)
            Animator.Unaim(this, animDuration, animDelay);
    }
}
